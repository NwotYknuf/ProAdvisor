<html>
<head>
	<title>Initialisation de la base de données</title>
	<meta charset="utf-8" />
  	<link href="style.css" rel="stylesheet" type="text/css" />
</head>

<body>
<div id="templatemo_header">
		<div id="templatemo_site_title">R-PRO <span>ADVISOR</span></div>
		<div id="templatemo_site_slogan">Find your renovation partner</div>
</div>

<div id="templatemo_container">
<?php
	include("Parametres.php");
	include("Fonctions.inc.php");
    // Connexion au serveur MySQL
	$mysqli=mysqli_connect($host,$user,$pass) or die("Problème de création de la base :".mysqli_error());
	mysqli_select_db($mysqli,$base) or die("Impossible de sélectionner la base : ".$base);
	if(isset($_GET["EntrepriseCherchee"])){
		$EntrepriseCherchee = mysqli_real_escape_string($mysqli, $_GET["EntrepriseCherchee"]); 	
		$EntrepriseCherchee = str_replace("_", " ", $EntrepriseCherchee);
		$EntrepriseCherchee = str_to_noaccent($EntrepriseCherchee);
	}
	//recuperation de l'avis avec le titre correspondant dans la base de données
	$result = query($mysqli, "SELECT `User`, `Entreprise`, `Note`, `Commentaire` 
		FROM Notes
		WHERE (`Entreprise` = '$EntrepriseCherchee')
		;");
	//affichage du cocktail avec toutes ses caracteristiques
	foreach ($result as $EntrepriseCherchee){
		echo "<h3><u>".$EntrepriseCherchee["Entreprise"].":</u></h3>";
		affichageCaracteristiques($EntrepriseCherchee);//afficher les autres caractéristiques		
	}
	
	//Cette fonction enlève les accents et les caractères spéciaux d'un string donné
	function str_to_noaccent($str)
	{
	    $url = $str;
	    $url = preg_replace('#Ç#', 'C', $url);
	    $url = preg_replace('#ç#', 'c', $url);
	    $url = preg_replace('#è|é|ê|ë#', 'e', $url);
	    $url = preg_replace('#È|É|Ê|Ë#', 'E', $url);
	    $url = preg_replace('#à|á|â|ã|ä|å#', 'a', $url);
	    $url = preg_replace('#@|À|Á|Â|Ã|Ä|Å#', 'A', $url);
	    $url = preg_replace('#ì|í|î|ï#', 'i', $url);
	    $url = preg_replace('#Ì|Í|Î|Ï#', 'I', $url);
	    $url = preg_replace('#ð|ò|ó|ô|õ|ö#', 'o', $url);
	    $url = preg_replace('#Ò|Ó|Ô|Õ|Ö#', 'O', $url);
	    $url = preg_replace('#ù|ú|û|ü#', 'u', $url);
	    $url = preg_replace('#Ù|Ú|Û|Ü#', 'U', $url);
	    $url = preg_replace('#ý|ÿ#', 'y', $url);
	    $url = preg_replace('#Ý#', 'Y', $url);
	    $url = preg_replace('#ñ#', 'n', $url);
	    $url = preg_replace('/[^a-z(:).\d ]/i', '', $url);
	    return ($url);//le string donné en parametre est retourné sans accents ni caractères spéciaux
	}

	function affichageCaracteristiques($EntrepriseCherchee)
	{
		echo "<ul>";
		//Parcours du Cocktail par caracteristiques
		foreach ($EntrepriseCherchee as $key => $Caracteristique)
		{
			if ($key == "index"){
				//si on est dans l'index, un array
				$Caracteristique = explode(",", $Caracteristique);
				array_pop($Caracteristique);//isoler chaque element de l'index
				echo "Index: </br>";
				foreach ($Caracteristique as $element) {
					//parcourir ces elements et les afficher un par un dans une liste
					echo $element."</br>";
				}
				echo "</br>";
			}
			else if($key != "titre")
			{
				//si on est pas dans l'index ni le titre(qui est déjà affiché), afficher la caracteristique où on se situe
				echo $key." : ".$Caracteristique."</br></br>";
			}
		}
		echo "</ul>";
	}
?>

</div>
</body>
</html>