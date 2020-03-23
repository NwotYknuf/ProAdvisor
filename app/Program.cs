using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace ProAdvisor.app {
    class Program {

        public static List<Entreprise> trouverEntreprises(List<Bot> bots, string quoi, (double lat, double lon)ou) {

            List<Entreprise> entreprises = new List<Entreprise>();

            foreach (Bot bot in bots) {

                if (bot is ISourceEntreprise) {
                    ISourceEntreprise scrapper = bot as ISourceEntreprise;

                    try {
                        entreprises.AddRange(scrapper.findEntreprise(quoi, ou.lat, ou.lon).Result);
                    } catch (AggregateException ae) {
                        Console.WriteLine(ae.InnerException.Message + ae.InnerException.StackTrace);
                    }
                }

            }

            return entreprises;

        }

        public static DateTime trouverDernierCommentaire(Bot bot, Entite entite, string connectionString) {

            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand cmd = connection.CreateCommand();

            if (entite is Service) {
                cmd.CommandText = @"
                SELECT max(date) from commentaire 
                where source like('" + bot.source + "') and url_service like('" + entite.id + "')";
            } else {
                cmd.CommandText = @"
                SELECT max(date) from commentaire
                where source like('" + bot.source + "') and siret like('" + entite.id + "')";
            }

            MySqlDataReader reader = cmd.ExecuteReader();

            reader.Read();

            DateTime res = new DateTime(2010, 1, 1);

            if (!reader.IsDBNull(0)) {
                res = reader.GetDateTime(0);
            }

            connection.Close();

            return res;
        }

        public static List<Review> trouverReviews(List<Bot> bots, Entite entite, string connectionString) {

            List<Review> reviews = new List<Review>();

            foreach (Bot bot in bots) {

                if (bot is ISourceInfo) {

                    DateTime dateLimite = trouverDernierCommentaire(bot, entite, connectionString);

                    try {
                        reviews.AddRange((bot as ISourceReview).findReviews(entite, dateLimite).Result);
                    } catch (AggregateException ae) {

                        if (!(ae.InnerException is EntrepriseInconnueException) &&
                            !(ae.InnerException is PasDeCommentaireException)) {
                            Console.WriteLine(ae.InnerException.Message + ae.InnerException.StackTrace);
                        }
                    }

                }
            }

            return reviews;

        }

        public static List<Entite> getEntites(string connectionString) {
            List<Entite> entites = new List<Entite>();
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "Select * from entreprise";

            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) {
                string siret = reader.GetString("siret");
                string nom = reader.GetString("nom");
                string url = reader.GetString("url");
                string adresse = reader.GetString("adresse");

                Entreprise e = new Entreprise(siret, nom, url, adresse, null, null);
                entites.Add(e);
            }
            reader.Close();

            cmd = connection.CreateCommand();
            cmd.CommandText = "Select * from service_web";

            reader = cmd.ExecuteReader();

            while (reader.Read()) {
                string url = reader.GetString("url_service");
                string nom = reader.GetString("nom");
                Service s = new Service(url, nom);
                entites.Add(s);
            }
            reader.Close();

            connection.Close();

            return entites;
        }

        public static void test() {
            HttpClient client = new HttpClient();

            HttpResponseMessage response = client.GetAsync("https://www.pagesjaunes.fr").Result;

            if (response.StatusCode == HttpStatusCode.Forbidden) {
                Console.WriteLine("Acces à PagesJaunes impossible");
            }

            if (response.StatusCode == HttpStatusCode.OK) {
                Console.WriteLine("Acces à PagesJaunes ok");
            }

        }

        public static void entreprise(string inputFolder, string outputFolder) {

            List<Bot> bots = new List<Bot>(ChildClassEnumerator.GetEnumerableOfType<Bot>());

            List<Entreprise> entreprises = new List<Entreprise>();

            List<string> typesEntreprise = new List<string>();
            StreamReader sr = new StreamReader(inputFolder + "TypesEntreprises.txt");
            string line;
            while ((line = sr.ReadLine()) != null) {
                typesEntreprise.Add(line);
            }
            sr.Close();

            List < (double lat, double lon) > positions = new List < (double lat, double lon) > ();
            sr = new StreamReader(inputFolder + "Coordonnees.txt");
            while ((line = sr.ReadLine()) != null) {
                string[] pos = line.Split(",");
                positions.Add((Double.Parse(pos[0]), Double.Parse(pos[1])));
            }
            sr.Close();

            foreach (string typeEntreprise in typesEntreprise) {
                foreach ((double lat, double lon)pos in positions) {
                    entreprises.AddRange(trouverEntreprises(bots, typeEntreprise, pos));
                }
            }

            saveJson(outputFolder, "entreprises.json", entreprises);

            foreach (Bot bot in bots) {
                bot.destroy();
            }

        }

        public static void saveJson(string folder, string filename, object obj) {
            if (!Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }

            StreamWriter sw = new StreamWriter(folder + filename);
            sw.Write(JsonConvert.SerializeObject(obj));
            sw.Close();
        }

        public static void commentaires(string outputFolder, string connectionString) {

            List<Bot> bots = new List<Bot>(ChildClassEnumerator.GetEnumerableOfType<Bot>());

            List<Entite> entites = getEntites(connectionString);
            List<Review> reviews = new List<Review>();

            foreach (Entite entite in entites) {
                reviews.AddRange(trouverReviews(bots, entite, connectionString));
            }

            saveJson(outputFolder, "Commentaires.json", reviews);

            foreach (Bot bot in bots) {
                bot.destroy();
            }

        }

        public static void infos(string outputFolder, string connectionString) {

            List<Bot> bots = new List<Bot>(ChildClassEnumerator.GetEnumerableOfType<Bot>());

            List<Entite> entites = getEntites(connectionString);

            Dictionary<string, List<Info>> infos = new Dictionary<string, List<Info>>();

            foreach (Bot bot in bots) {

                if (bot is ISourceInfo) {

                    foreach (Entite entite in entites) {

                        try {
                            if (!infos.ContainsKey(bot.source)) {
                                infos[bot.source] = new List<Info>();
                            }
                            infos[bot.source].Add((bot as ISourceInfo).findInfos(entite).Result);
                        } catch (AggregateException ae) {

                            if (!(ae.InnerException is EntrepriseInconnueException)) {
                                Console.WriteLine(ae.InnerException.Message + ae.InnerException.StackTrace);
                            }
                        }
                    }
                }
            }

            foreach (var pair in infos) {
                saveJson(outputFolder, pair.Key + "_info.json", pair.Value);
            }

        }

        public static void Main(string[] args) {

            if (args.Length > 0) {

                string inputFolder;
                string outputFolder;
                string connectionString;

                switch (args[0]) {
                    case "entreprises":
                        inputFolder = args[1];
                        outputFolder = args[2];
                        entreprise(inputFolder, outputFolder);
                        break;
                    case "commentaires":
                        outputFolder = args[1];
                        connectionString = args[2];
                        commentaires(outputFolder, connectionString);
                        break;
                    case "infos":
                        outputFolder = args[1];
                        connectionString = args[2];
                        infos(outputFolder, connectionString);
                        break;
                    case "test":
                        test();
                        break;
                    default:
                        Console.WriteLine("Mode inconnu");
                        break;
                }
            } else {
                Console.WriteLine("Pas d'arguments donnés");
            }
        }
    }
}