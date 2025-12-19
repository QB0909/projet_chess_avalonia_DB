# Rapport Projet Programmation Orientée Objet

Dans le cadre du cours de Programmation orientée objet nous avons du faire un programme en c# afin d'exercer nos connaissances.
Le projet est un logiciel destiné à gérer des matchs d’une fédération d’échecs et sera utilisé par le personnel administratif de la fédération.
Celui-ci entrera les informations manuellement dans l'application.

La fonctionnalité suplémentaire que nous avons choisis est une page reprenant différentes statistiques tel que l'historique des Elos d'un joueur, son elo maximum, minimum et moyen. Nous faisons donc des requêtes dans un fichier nommé `data` afin de récupérer les données utiles à la page.

## Diagramme de Classe
![Diagramme de Classe](chemin/vers/image_classe.png)

## Diagramme de Séquences
![Diagramme de Séquences](chemin/vers/image_sequences.png)

## Diagramme d'Activité
![Diagramme d'Activité](chemin/vers/image_activite.png)

Le projet est moyennement adaptable à une autre fédération : les joueurs sont déterminés par la couleur de leurs pièces et leur niveau est déterminé par leur elo.
Pour modifier l'appel des joueurs et leur classement il faudrait modifier le code déjà existant.
Pour le reste, on pourrait l'adapter en ajoutant des scripts dans `ModelViews` et dans `Models` mais il faudra par contre modifier `MainWindow.axaml` directement.

## Principes SOLID utilisés :

### Single Responsibility Principle

Chaque classe ou méthode n'a qu'une seule fonctionnalité, tout est découpé en blocs ne faisant qu'une seule chose.

### Dependency Inversion Principle

Nous respectons simplement le pattern MVVM.
Par exemple, L'interface `MainWindow.axaml` dépend du ViewModel mais pas l'inverse.

## En conclusion,
