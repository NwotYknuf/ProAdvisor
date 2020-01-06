<html>
<head>
    <title>Projet synthèse</title>
    <meta charset="utf-8" />
  <link href="style.css" rel="stylesheet" type="text/css" />
</head>

<body>
    <?php
        include("Parametres.php");
          include("Fonctions.inc.php");
          $mysqli=mysqli_connect($host,$user,$pass) or
        die("Problème de création de la base :".mysqli_error($mysqli));
    ?>
    <div id="templatemo_header">
        <div id="templatemo_site_title">R-PRO <span>ADVISOR</span></div>
        <div id="templatemo_site_slogan">Find your renovation partner</div>
</div>
    <main>
        <form method="POST" action ="">
        <p>Nom de l'entreprise ou du professionel:</p>
        <input type="text" name="recherche">
        <p>Dans quelle catégorie recherchez-vous?</p>
          <fieldset>
            <input type="radio" id="chauffage" name="Categorie" value="Chauffage">
            <label for="chauffage">Chaufage</label> 
            <input type="radio" id="carrelage" name="Categorie" value="Carrelage">
            <label for="carrelage">Carrelage</label>
            <input type="radio" id="electronique" name="Categorie" value="Électronique">
            <label for="electronique">Électronique</label> 
          </fieldset>
          <input type="submit" name="submit" value="Recherher">
        </form>
        
        <?php
        
        if(!isset($_GET['p']))
        {
          include "install.php";//creer la bdd si pas deja fait
          
        }
        
        if(isset($_POST["submit"])){
            if(isset($_POST["recherche"])){
                $search = mysqli_real_escape_string($mysqli, $_POST["recherche"]); 
            }
            else{
                $search = "";
            }
            if(isset($_POST["categorie"])){
                $cat = mysqli_real_escape_string($mysqli, $_POST["categorie"]); 
            }
            else{
                $cat = "";
            }

            if($search == ""){
                $result = query($mysqli, "SELECT `nomEntreprise`, `categorie`
                    FROM Entreprise
                    WHERE `categorie` = 'ALL'
                    OR `categorie` = '$cat'
                    ;");
            }
            else{
                $result = query($mysqli, "SELECT `nomEntreprise`, `categorie`
                    FROM Entreprise
                    WHERE '$search' = `nomEntreprise`
                    AND `categorie` = 'ALL'
                    OR `categorie` = '$cat'
                    ;");
            }
            
            if (mysqli_num_rows($result)==0) 
            { 
                echo "Malheureusement aucune entreprise correspond à votre recherche, verifiez l' orthographe de vos criteres</br>";
                mysqli_select_db($mysqli,$base) or die("Impossible de sélectionner la base : ".$base);
              $result = query($mysqli, "SELECT `nomEntreprise`, `categorie` FROM Entreprise");
              echo "Voici toutes les entreprises:";
              echo "</br>";
              foreach ($result as $EntrepriseCherchee)
              {
                $titre = $EntrepriseCherchee["nomEntreprise"];
                $ingredients = trim($EntrepriseCherchee["categorie"]);
                echo "<ul>";
                echo "$titre</br>";
                echo "Categorie: $ingredients";
                echo "</ul>";
              }
            }
            //Sinon afficher les résultats
            else
            {
                echo "Voici les résultats de votre recherche";
                show2($result);
            }
        }
        else{
            $result = query($mysqli, "SELECT `nomEntreprise`, `categorie` FROM Entreprise");
              //affichage des ingredients et titres des Cosktails avec lien vers EntrepriseCherchee detaillé
              echo "Voici toutes les entreprises:";
              echo "</br>";
              foreach ($result as $EntrepriseCherchee)
              {
                $titre = $EntrepriseCherchee["nomEntreprise"];
                $ingredients = trim($EntrepriseCherchee["categorie"]);
                echo "<ul>";
                echo "$titre</br>";
                echo "Categorie: $ingredients";
                echo "</ul>";
              }
        }

        function show($result){
            foreach ($result as $titreEntrepriseCherchee){
                foreach ($result as $EntrepriseCherchee)
              {
                $titre = $EntrepriseCherchee["nomEntreprise"];
                $ingredients = trim($EntrepriseCherchee["categorie"]);
                echo "<ul>";
                echo "$titre</br>";
                echo "Categorie: $ingredients";
                echo "</ul>";
              }
            }
        }

        function show2($result){
        foreach ($result as $titreEntrepriseCherchee){
                foreach ($titreEntrepriseCherchee as $value){
                    echo '<li><a href = "EntrepriseCherchee.php?EntrepriseCherchee='.$value.'"/>'.$value.'</a></li>';
                    break;
                }
            }
        }
?>
    </main>
</body>

</html>