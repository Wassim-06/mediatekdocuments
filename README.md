Dépôt d'origine
Le dépôt d'origine contenant la présentation complète de l'application est disponible ici :
[👉 Dépôt MediatekDocuments original]
(https://github.com/CNED-SLAM/MediaTekDocuments)

🚀 Missions et Tâches réalisées
Mission 1 : Gérer les documents
Analyse et gestion des documents (livres, DVD, revues) de la médiathèque.

Ajout de fonctionnalités pour rechercher, filtrer, trier et afficher les documents.

Mission 2 : Gérer les commandes
Tâche 1 : Gestion des commandes de livres ou DVD
Développement complet de la gestion des commandes de livres et DVD (ajout, suppression, suivi).

Tâche 2 : Gestion des abonnements de revues
Implémentation de la gestion des commandes et des abonnements aux revues.

Mission 3 : Gestion du suivi des exemplaires
Suivi des états des exemplaires reçus (état d'usure, date d'achat, etc.).

Mise à jour automatique de l'état lors de la livraison.

Mission 4 : Authentification
Mise en place de l'authentification des utilisateurs avec l'API REST.

Mission 5 : Sécuriser, Qualité, Intégration de logs
Tâche 1 : Sécuriser la connexion à l'API
Ajout de la gestion sécurisée des accès API (via .env).

Tâche 2 : Gestion des erreurs
Ajout des logs automatiques sur les erreurs dans un fichier logs.txt.

Tâche 3 : Intégration des logs dans la DAL (Access.cs)
Mission 6 : Tester et documenter
Tâche 1 : Tests unitaires
Réalisation de tests unitaires sur toutes les classes du modèle (model).

Tâche 2 : Tests API avec Postman
Construction d'une collection Postman pour tester toutes les routes de l'API.

Tâche 3 : Documentation technique
Génération de la documentation C# (Sandcastle).

Génération de la documentation API (PHPDocumentor).

Mission 7 : Déploiement et sauvegarde
Tâche 1 : Déploiement de l'API
Déploiement de l'API REST et de la BDD en ligne sur AlwaysData.

Tests en ligne de l'API via Postman.

Tâche 2 : Mise en place d'une sauvegarde automatique
Programmation d'une tâche CRON pour la sauvegarde journalière de la base de données.

🛠️ Mode opératoire pour installer et utiliser l'application en local
1. Prérequis
Windows 10 ou supérieur

.NET Framework 4.7.2 ou supérieur

Accès Internet (obligatoire pour accéder à l'API distante)

2. Installation de l'application
Télécharger le fichier SetupMediatekDocuments.msi disponible dans le dossier installeur/ du dépôt.

![image](https://github.com/user-attachments/assets/9bf81b8b-01e3-43dc-b532-41a3ff8ed26a)

Lancer l'installeur.

Par défaut, l’application sera installée dans :
D:\Cned\Atelier2\ApplicationInstaller\

Finaliser l'installation en suivant les étapes.

3. Lancer l'application
Ouvrir MediatekDocuments depuis votre menu Démarrer ou via le dossier d'installation.

![image](https://github.com/user-attachments/assets/51a4b537-1275-49d9-9eba-1ab3b9a1fd25)

Se connecter à l’API distante :

➔ https://atelier2mediatekformation.alwaysdata.net/

![image](https://github.com/user-attachments/assets/d542c39a-ef99-4e07-9ec9-9c90249fd8af)

Utiliser toutes les fonctionnalités : gestion des livres, DVD, revues, abonnements, commandes.

4. Restauration de la BDD (en cas de besoin)
La sauvegarde automatique crée un fichier .sql chaque jour.

Pour restaurer, importer simplement le fichier .sql correspondant via phpMyAdmin sur AlwaysData.

