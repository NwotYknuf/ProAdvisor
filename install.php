<?php
	//verifier si la base existe, si non, la creer et inserer les donnees
	$ex = mysqli_select_db($mysqli, $base);
	if($ex == true){
	echo "";
	
}
else{
	query($mysqli,'CREATE DATABASE IF NOT EXISTS '.$base);
	//connection a la bdd
	mysqli_select_db($mysqli,$base) or die("Impossible de sélectionner la base : ".$base);
	include("Donnees.inc.php");//inclusion du fichier contenant les arrays
   
	//Creation de la table pour les recettes avec le numero, titre, les ingredients, la preparation et les index
	query($mysqli, "CREATE TABLE IF NOT EXISTS Entreprise(
		`nomEntreprise`VARCHAR(200),
	    `categorie` VARCHAR(200)
	    );");
	query($mysqli, "CREATE TABLE IF NOT EXISTS Notes(
		`User`VARCHAR(200),
	    `Entreprise` VARCHAR(200),
	    `Note` INT(2),
	    `Commentaire` VARCHAR(200)
	    );");
	
	//Insertion des valeurs dans la table recette
	foreach($Entreprises as $num => $details)
	{
		//parcourir toutes les recettes
		$toadd = array();//dans ce tableau seront mis les elements
		foreach($details as $description => $rubrique)
		{
			//parcourir la recette

			$name = str_to_noaccent($details[$description], $description);
			array_push($toadd, $name);
		}
		//Ajout des données dans l' ordre
		query($mysqli, "INSERT INTO Entreprise(`nomEntreprise`, `categorie`) 
			VALUES(
			'$toadd[0]',
			'$toadd[1]'
			);");
	}

	foreach($Notes as $num => $details)
	{
		//parcourir toutes les recettes
		$toadd = array();//dans ce tableau seront mis les elements
		foreach($details as $description => $rubrique)
		{
			//parcourir la recette

			$name = str_to_noaccent($details[$description], $description);
			array_push($toadd, $name);
		}
		//Ajout des données dans l' ordre
		query($mysqli, "INSERT INTO Notes(`User`, `Entreprise`, `Note`, `Commentaire`) 
			VALUES(
			'$toadd[0]',
			'$toadd[1]',
			'$toadd[2]',
			'$toadd[3]'
			);");
	}
}	
function str_to_noaccent($str, $description)
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
	    if($description!= "ingredients"){
	    	$url = preg_replace('/[^a-z(:).\d ]/i', '', $url);
	    }
	    else{
	    	$url = preg_replace('/[^a-z(:).\d ]/i', ', ', $url);
	    }
	    return ($url);
	}
?>
