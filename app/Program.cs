using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenQA.Selenium;

namespace ProAdvisor.app {
    class Program {
        public static void Main(string[] args) {

            PagesJaunesScrapper scrapper = new PagesJaunesScrapper();

            try {
                List<Entreprise> entreprises = scrapper.findEntreprise("").Result;
            } catch (AggregateException ae) {
                throw ae.InnerException;
            }

        }
    }
}