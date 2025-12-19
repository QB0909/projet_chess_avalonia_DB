titre -> Rapport Projet Programmation Orientée Objet 

paragraphe -> Dans le cadre du cours de Programmation orientée objet nous avons du faire un programme en c# afin d'exercer nos connaissances. 
              Le projet est un logiciel destiné à gérer des matchs d’une fédération d’échecs et sera utilisé par le personnel administratif de la fédération. 
              Celui-ci entrera les informations manuellement dans l'application. 

paragraphe -> La fonctionnalité suplémentaire que nous avons choisis est une page reprenant différentes statistiques tel que l'historique des Elos d'un joueur, son elo maximum, minimum et moyen. Nous faisons donc des requêtes dans un fichier nommé data afin 
              de récupérer les données utiles à la page.



titre -> Diagramme de Classe





titre -> Diagramme de Séquences





titre -> Diagramme d'Activité






paragraphe -> Le projet est moyennement adaptable à une autre fédération : les joueurs sont déterminés par la couleur de leurs pièces et leur niveau est déterminé par leur elo. 
              Pour modifier l'appel des joueurs et leur classement il faudrait modifier le code déjà existant. 
              Pour le reste, on pourrait l'adapter en ajoutant des scripts dans ModelViews et dans Models mais il faudra par contre modifier MainWindow.axaml directement.



titre -> Principes SOLID utilisés : 


sous titre -> Single Responsibility Principle

Chaque classe ou méthode n'a qu'une seule fonctionnalité, tout est découpé en blocs ne faisant qu'une seule chose. 

sous titre -> Dependency Inversion Principle

Nous respectons simplement le pattern MVVM. 
L'interface donc "MainWindow.axaml" dépend du ViewModel mais pas l'inverse. 
  







































