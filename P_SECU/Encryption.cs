/// ETML
/// Auteur: Félix Sierro
/// Date: 19.03.2024
/// Description: Application gérant des mots de passe et qui permet de les encrypter et de les decrypter

using System;
using System.IO;
using System.Text;

namespace P_SECU
{
    internal class Encryption
    {
        // Déclaration du mot de passe principal.
        private string masterPsw = "";

        // Chemin d'accès au fichier où est stocké le mot de passe principal.
        private const string masterPswFilePath = @"..\..\..\masterPsw\masterpsw.txt";

        // Chemin d'accès au répertoire contenant les fichiers texte des mots de passe.
        private const string folderPath = @"..\..\..\psw\";

        /// <summary>
        /// Charge l'application et vérifie le mot de passe principal.
        /// </summary>
        public void Load()
        {
            // Charge le mot de passe principal depuis le fichier.
            LoadMasterPassword();

            // Vérifie si aucun mot de passe principal n'est enregistré.
            if (string.IsNullOrEmpty(masterPsw))
            {
                // Aucun mot de passe principal enregistré, demande à l'utilisateur d'en saisir un.
                Console.WriteLine("Aucun mot de passe principal enregistré.");
                Console.Write("\nVeuillez entrer un nouveau mot de passe principal : ");
                masterPsw = ReadPSW();
                Console.Write("\n\n");

                // Enregistre le nouveau mot de passe principal.
                SaveMasterPassword();
            }
            
            // Compteur pour les tentatives de saisie du mot de passe.
            int count = 0;
            string input;
            Console.Write("Veuillez entrer le mot de passe principal : ");

            // Boucle pour saisir et vérifier le mot de passe principal.
            do
            {
                input = ReadPSW();
                if (input == masterPsw)
                {
                    // Si le mot de passe est correct, affiche le menu principal.
                    Console.Clear();
                    MainMenu();
                }
                else
                {
                    // Si le mot de passe est incorrect, incrémente le compteur et invite à réessayer.
                    count++;
                    Console.Clear();
                    Console.Write("Mot de passe incorrect, veuillez réessayer : ");
                }

                // Si le nombre de tentatives atteint 3, ferme l'application.
                if (count == 3)
                {
                    Console.Clear();
                    Console.Write("Trois tentatives incorrectes, l'application va se fermer");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            } while (input != masterPsw);
        } // Load

        /// <summary>
        /// Charge le mot de passe principal à partir du fichier chiffré.
        /// </summary>
        private void LoadMasterPassword()
        {
            // Vérifie si le fichier du mot de passe principal existe.
            if (File.Exists(masterPswFilePath))
            {
                // Lit le mot de passe principal chiffré depuis le fichier.
                string encryptedMasterPsw = File.ReadAllText(masterPswFilePath);

                // Déchiffre le mot de passe principal en utilisant la clé par défaut.
                masterPsw = VigenereDecrypt(encryptedMasterPsw, "default_key");
            }
        } // LoadMasterPassword

        /// <summary>
        /// Enregistre le mot de passe principal dans un fichier chiffré.
        /// </summary>
        private void SaveMasterPassword()
        {
            // Chiffre le mot de passe principal en utilisant la clé par défaut.
            string encryptedMasterPsw = VigenereEncrypt(masterPsw, "default_key");

            // Écrit le mot de passe chiffré dans le fichier spécifié.
            File.WriteAllText(masterPswFilePath, encryptedMasterPsw);
        } // SaveMasterPassword

        /// <summary>
        /// Affiche le menu principal et gère les choix de l'utilisateur.
        /// </summary>
        private void MainMenu()
        {
            // Boucle infinie pour afficher le menu à chaque fois qu'une action est terminée.
            while (true)
            {
                // Affichage du menu principal.
                Console.WriteLine("*************************************");
                Console.WriteLine("Sélectionnez une action");
                Console.WriteLine("1. Consulter un mot de passe");
                Console.WriteLine("2. Ajouter un mot de passe");
                Console.WriteLine("3. Supprimer un mot de passe");
                Console.WriteLine("4. Modifier un mot de passe");
                Console.WriteLine("5. Changer le mot de passe principal");
                Console.WriteLine("6. Quitter le programme");
                Console.WriteLine("*************************************\n\n");
                Console.Write("Faites votre choix : ");

                // Lecture de la sélection de l'utilisateur.
                string selection = Console.ReadLine();

                // Switch pour gérer les différentes actions en fonction de la sélection de l'utilisateur.
                switch (selection)
                {
                    case "1":
                        Console.Clear();
                        CheckPSW(); // Affiche les mot de passe stocké
                        break;
                    case "2":
                        Console.Clear();
                        AddPSW(); // Ajoute un mot de passe
                        break;
                    case "3":
                        Console.Clear();
                        DeletePSW(); // Supprime un mot de passe
                        break;
                    case "4":
                        Console.Clear();
                        ModifyPSW(); // Modifie un mot de passe
                        break;
                    case "5":
                        Console.Clear();
                        ChangeMasterPassword(); // Modifie le mot de passe principal
                        break;
                    case "6":
                        Environment.Exit(0); // Quitte l'application
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Entrez un chiffre entre 1 et 6\n");
                        break;
                }
            } // MainMenu
        }

        /// <summary>
        /// Affiche les mots de passe stockés et permet à l'utilisateur d'en consulter un.
        /// </summary>
        private void CheckPSW()
        {
            Console.Clear();

            Console.WriteLine("Consulter un mot de passe :");

            // Obtient la liste des fichiers texte dans le dossier des mots de passe.
            string[] files = Directory.GetFiles(folderPath, "*.txt");

            if (files.Length == 0)
            {
                // Si aucun mot de passe n'est enregistré, affiche un message et retourne au menu principal.
                Console.WriteLine("\nAucun mot de passe enregistré.");
                Console.Write("\nAppuyez sur Enter pour revenir au menu principal.");
                Console.ReadLine();
                Console.Clear();
                MainMenu();
                return;
            }

            // Affiche les options de consultation des mots de passe.
            Console.WriteLine("\n1. Retour au menu principal");

            int count = 2;
            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                Console.WriteLine($"{count}. {fileName}");
                count++;
            }
            Console.WriteLine("*************************************");

            int selectedIndex;
            while (true)
            {
                // Demande à l'utilisateur de choisir un mot de passe à consulter.
                Console.Write("\nFaites votre choix : ");
                string selection = Console.ReadLine();

                if (selection == "1")
                {
                    // Si l'utilisateur choisit de revenir au menu principal, retourne au menu principal.
                    Console.Clear();
                    MainMenu();
                    return;
                }
                else if (int.TryParse(selection, out selectedIndex) && selectedIndex >= 2 && selectedIndex < count)
                {
                    // Si l'utilisateur choisit un mot de passe valide, sort de la boucle.
                    break;
                }
                else
                {
                    // Si la sélection est invalide, affiche un message d'erreur.
                    Console.Write($"\nEntrez un chiffre valide entre 1 et {count - 1}\n");
                }
            }

            Console.Clear();
            // Charge les données du mot de passe sélectionné.
            string[] lines = File.ReadAllLines(files[selectedIndex - 2]);
            string name = Path.GetFileNameWithoutExtension(files[selectedIndex - 2]);
            string url = lines[0];
            string login = lines[1];
            string encryptedPassword = lines[2];
            // Déchiffre le mot de passe.
            string decryptedPassword = VigenereDecrypt(encryptedPassword, masterPsw);

            // Affiche les informations du mot de passe.
            Console.WriteLine($"Nom du site : {name}");
            Console.WriteLine($"URL : {url}");
            Console.WriteLine($"Login : {login}");
            Console.WriteLine($"Mot de passe : {decryptedPassword}");

            // Attend une entrée pour revenir au menu principal.
            Console.Write("\nAppuyez sur Enter pour revenir au menu principal.");
            Console.ReadLine();
            Console.Clear();
            MainMenu();
        } // CheckPSW

        /// <summary>
        /// Ajoute un nouveau mot de passe avec les informations associées.
        /// </summary>
        private void AddPSW()
        {
            // Demande à l'utilisateur de saisir les informations pour le nouveau mot de passe
            Console.Write("Nom du site : ");
            string name = Console.ReadLine();
            Console.Write("URL : ");
            string url = Console.ReadLine();
            Console.Write("Login : ");
            string login = Console.ReadLine();
            Console.Write("Mot de passe : ");
            string psw = Console.ReadLine();

            // Chiffre le mot de passe avec le mot de passe principal
            string encryptedPsw = VigenereEncrypt(psw, masterPsw);

            // Crée une chaîne de caractères avec les informations du mot de passe
            string data = $"{url}\n{login}\n{encryptedPsw}";

            // Écrit les informations dans un fichier texte portant le nom du site
            File.WriteAllText(Path.Combine(folderPath, $"{name}.txt"), data);

            // Affiche un message de confirmation
            Console.WriteLine("\nLe mot de passe a été ajouté avec succès");

            // Attente de l'appui sur Enter pour revenir au menu principal
            Console.Write("\nAppuyez sur Enter pour revenir au menu principal");
            Console.ReadLine();
            Console.Clear();
            MainMenu();
        } // AddPSW

        /// <summary>
        /// Supprime un mot de passe existant.
        /// </summary>
        private void DeletePSW()
        {
            // Nettoie la console et affiche le titre de la fonctionnalité
            Console.Clear();
            Console.WriteLine("Supprimer un mot de passe :");

            // Récupère la liste des fichiers de mot de passe enregistrés
            string[] files = Directory.GetFiles(folderPath, "*.txt");

            // Vérifie s'il n'y a aucun mot de passe enregistré
            if (files.Length == 0)
            {
                Console.WriteLine("\nAucun mot de passe enregistré.");
                Console.Write("\nAppuyez sur Enter pour revenir au menu principal.");
                Console.ReadLine();
                Console.Clear();
                MainMenu();
                return;
            }

            // Affiche la liste des mots de passe enregistrés avec leur numéro correspondant
            Console.WriteLine("Voici les mots de passe enregistrés :\n");

            int count = 1;
            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                Console.WriteLine($"{count}. {fileName}");
                count++;
            }

            // Demande à l'utilisateur de choisir le numéro du mot de passe à supprimer
            Console.Write("\nEntrez le numéro du mot de passe que vous souhaitez supprimer (ou 0 pour annuler) : ");

            // Vérifie si l'entrée de l'utilisateur est un numéro valide
            if (int.TryParse(Console.ReadLine(), out int selectedNumber) && selectedNumber > 0 && selectedNumber <= files.Length)
            {
                // Demande confirmation pour la suppression du mot de passe sélectionné
                Console.Write($"\nVoulez-vous vraiment supprimer le mot de passe pour {Path.GetFileNameWithoutExtension(files[selectedNumber - 1])} ? (O/N) : ");
                string confirmation = Console.ReadLine();

                // Supprime le fichier du mot de passe si l'utilisateur confirme
                if (confirmation.ToLower() == "o" || confirmation.ToLower() == "oui")
                {
                    File.Delete(files[selectedNumber - 1]);
                    Console.WriteLine("\nMot de passe supprimé avec succès.");
                }
                else
                {
                    Console.WriteLine("\nSuppression annulée.");
                }
            }
            else if (selectedNumber == 0)
            {
                Console.WriteLine("\nSuppression annulée.");
            }
            else
            {
                Console.WriteLine("\nSélection invalide.");
            }

            // Attente de l'appui sur Enter pour revenir au menu principal
            Console.Write("\nAppuyez sur Enter pour revenir au menu principal.");
            Console.ReadLine();
            Console.Clear();
            MainMenu();
        } // DeletePSW

        /// <summary>
        /// Modifie un mot de passe existant.
        /// </summary>
        private void ModifyPSW()
        {
            Console.Clear();
            Console.WriteLine("Modifier un mot de passe :");

            // Obtient la liste des fichiers texte dans le dossier des mots de passe.
            string[] files = Directory.GetFiles(folderPath, "*.txt");

            if (files.Length == 0)
            {
                // Si aucun mot de passe n'est enregistré, affiche un message et retourne au menu principal.
                Console.WriteLine("\nAucun mot de passe enregistré.");
                Console.Write("\nAppuyez sur Enter pour revenir au menu principal.");
                Console.ReadLine();
                Console.Clear();
                MainMenu();
                return;
            }

            // Affiche les noms des fichiers contenant les mots de passe enregistrés.
            Console.WriteLine("Voici les mots de passe enregistrés :\n");

            int count = 1;
            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                Console.WriteLine($"{count}. {fileName}");
                count++;
            }

            int selectedNumber;
            do
            {
                // Demande à l'utilisateur de choisir un mot de passe à modifier.
                Console.Write("\nEntrez le numéro du mot de passe que vous souhaitez modifier (ou 0 pour annuler) : ");
                if (int.TryParse(Console.ReadLine(), out selectedNumber) && selectedNumber >= 1 && selectedNumber <= files.Length)
                {
                    Console.Clear();

                    // Charge les données du fichier contenant le mot de passe sélectionné.
                    string[] lines = File.ReadAllLines(files[selectedNumber - 1]);
                    string name = Path.GetFileNameWithoutExtension(files[selectedNumber - 1]);
                    string url = lines[0];
                    string login = lines[1];
                    string encryptedPassword = lines[2];

                    // Affiche les informations du mot de passe sélectionné.
                    Console.WriteLine($"1. Nom du site : {name}");
                    Console.WriteLine($"2. URL : {url}");
                    Console.WriteLine($"3. Login : {login}");
                    Console.WriteLine($"4. Mot de passe : {VigenereDecrypt(encryptedPassword, masterPsw)}");

                    int numberSelected;
                    do
                    {
                        // Demande à l'utilisateur de choisir l'élément à modifier.
                        Console.Write("\nEntrez le numéro de l'élément que vous souhaitez modifier (ou 0 pour annuler) : ");
                        if (int.TryParse(Console.ReadLine(), out numberSelected) && numberSelected >= 1 && numberSelected <= 4)
                        {
                            string newValue;
                            switch (numberSelected)
                            {
                                case 1:
                                    string newFileName;
                                    string newFilePath;
                                    do
                                    {
                                        // Demande à l'utilisateur de saisir le nouveau nom.
                                        Console.Write("\nNouveau nom : ");
                                        newFileName = Console.ReadLine();
                                        newFilePath = Path.Combine(folderPath, $"{newFileName}.txt");
                                        if (File.Exists(newFilePath))
                                        {
                                            Console.WriteLine("\nErreur : Un fichier avec ce nom existe déjà. Veuillez choisir un autre nom.");
                                        }
                                    } while (File.Exists(newFilePath));

                                    // Renomme le fichier contenant le mot de passe.
                                    File.Move(files[selectedNumber - 1], newFilePath);
                                    Console.WriteLine("\nNom modifié avec succès.");
                                    File.Delete(files[selectedNumber - 1]);
                                    break;
                                case 2:
                                    // Demande à l'utilisateur de saisir la nouvelle URL.
                                    Console.Write("\nNouvelle URL : ");
                                    newValue = Console.ReadLine();
                                    lines[0] = newValue;
                                    Console.WriteLine("\nURL modifiée avec succès.");
                                    break;
                                case 3:
                                    // Demande à l'utilisateur de saisir le nouveau login.
                                    Console.Write("\nNouveau login : ");
                                    newValue = Console.ReadLine();
                                    lines[1] = newValue;
                                    Console.WriteLine("\nLogin modifié avec succès.");
                                    break;
                                case 4:
                                    // Demande à l'utilisateur de saisir le nouveau mot de passe et le chiffre.
                                    Console.Write("\nNouveau mot de passe : ");
                                    newValue = VigenereEncrypt(Console.ReadLine(), masterPsw);
                                    lines[2] = newValue;
                                    Console.WriteLine("\nMot de passe modifié avec succès.");
                                    break;
                            }

                            // Enregistre les modifications dans le fichier.
                            File.WriteAllLines(files[selectedNumber - 1], lines);
                        }
                        else if (numberSelected == 0)
                        {
                            Console.WriteLine("\nModification annulée.");
                        }
                        else
                        {
                            Console.WriteLine("\nSélection invalide. Veuillez sélectionner un chiffre entre 1 et 4.");
                        }
                    } while (numberSelected < 0 || numberSelected > 4);
                }
                else if (selectedNumber == 0)
                {
                    Console.WriteLine("\nModification annulée.");
                }
                else
                {
                    Console.WriteLine("\nSélection invalide. Veuillez sélectionner un chiffre valide.");
                }
            } while (selectedNumber < 0 || selectedNumber > files.Length);

            // Attend une entrée pour revenir au menu principal.
            Console.Write("\nAppuyez sur Enter pour revenir au menu principal.");
            Console.ReadLine();
            Console.Clear();
            MainMenu();
        } // ModifyPSW

        /// <summary>
        /// Change le mot de passe principal et réencrypte tous les mots de passe existants avec le nouveau mot de passe principal.
        /// </summary>
        private void ChangeMasterPassword()
        {
            Console.Clear();
            Console.Write("Entrez le mot de passe principal actuel : ");
            string currentPsw = ReadPSW();

            // Vérifie si le mot de passe actuel correspond au mot de passe principal enregistré.
            if (currentPsw == masterPsw)
            {
                Console.Write("\n\nEntrez le nouveau mot de passe principal : ");
                string newMasterPsw = ReadPSW();

                // Obtient la liste des fichiers texte contenant les mots de passe.
                string[] files = Directory.GetFiles(folderPath, "*.txt");

                // Parcours tous les fichiers pour réencrypter les mots de passe avec le nouveau mot de passe principal.
                foreach (string file in files)
                {
                    string[] lines = File.ReadAllLines(file);
                    string url = lines[0];
                    string login = lines[1];
                    string encryptedPassword = lines[2];

                    // Décrypte le mot de passe avec l'ancien mot de passe principal.
                    string decryptedPassword = VigenereDecrypt(encryptedPassword, masterPsw);
                    // Réencrypte le mot de passe avec le nouveau mot de passe principal.
                    string newEncryptedPassword = VigenereEncrypt(decryptedPassword, newMasterPsw);

                    // Construit une nouvelle chaîne de données avec le nouveau mot de passe encrypté.
                    string newData = $"{url}\n{login}\n{newEncryptedPassword}";
                    // Écrit la nouvelle chaîne de données dans le fichier.
                    File.WriteAllText(file, newData);
                }

                // Met à jour le mot de passe principal enregistré avec le nouveau mot de passe principal.
                masterPsw = newMasterPsw;
                // Sauvegarde le nouveau mot de passe principal dans le fichier.
                SaveMasterPassword();

                Console.WriteLine("\n\nLe mot de passe principal a été changé avec succès.");
            }
            else
            {
                // Affiche un message d'erreur si le mot de passe actuel est incorrect.
                Console.WriteLine("\nMot de passe incorrect.");
            }

            // Attend une entrée pour revenir au menu principal.
            Console.Write("\nAppuyez sur Enter pour revenir au menu principal.");
            Console.ReadLine();
            Console.Clear();
            MainMenu();
        } // ChangeMasterPassword

        /// <summary>
        /// Encrypte une chaîne de caractères en utilisant le chiffrement de Vigenère avec une clé donnée.
        /// </summary>
        /// <param name="input">La chaîne de caractères à encrypter.</param>
        /// <param name="key">La clé utilisée pour le chiffrement.</param>
        /// <returns>La chaîne de caractères encryptée.</returns>
        private string VigenereEncrypt(string input, string key)
        {
            // Vérifie si la clé est nulle ou vide.
            if (string.IsNullOrEmpty(key))
                throw new InvalidOperationException("La clé doit être initialisée avant l'encryption.");

            // Initialise un nouveau StringBuilder pour stocker la chaîne encryptée.
            StringBuilder encrypted = new StringBuilder(input.Length);

            // Parcourt chaque caractère de la chaîne d'entrée pour l'encrypter.
            for (int i = 0; i < input.Length; i++)
            {
                // Récupère le caractère de la chaîne d'entrée.
                char c = input[i];
                // Récupère le caractère de la clé correspondant à la position actuelle modulo la longueur de la clé.
                char k = key[i % key.Length];
                // Ajoute le caractère encrypté au StringBuilder.
                encrypted.Append((char)((c + k) % 256));
            }

            // Retourne la chaîne encryptée.
            return encrypted.ToString();
        } // VigenereEncrypt

        /// <summary>
        /// Déchiffre une chaîne de caractères en utilisant le chiffrement de Vigenère avec une clé donnée.
        /// </summary>
        /// <param name="input">La chaîne de caractères à déchiffrer.</param>
        /// <param name="key">La clé utilisée pour le déchiffrement.</param>
        /// <returns>La chaîne de caractères déchiffrée.</returns>
        private string VigenereDecrypt(string input, string key)
        {
            // Vérifie si la clé est nulle ou vide.
            if (string.IsNullOrEmpty(key))
                throw new InvalidOperationException("La clé doit être initialisée avant la decryption.");

            // Initialise un nouveau StringBuilder pour stocker la chaîne déchiffrée.
            StringBuilder decrypted = new StringBuilder(input.Length);

            // Parcourt chaque caractère de la chaîne d'entrée pour la déchiffrer.
            for (int i = 0; i < input.Length; i++)
            {
                // Récupère le caractère de la chaîne d'entrée.
                char c = input[i];
                // Récupère le caractère de la clé correspondant à la position actuelle modulo la longueur de la clé.
                char k = key[i % key.Length];
                // Ajoute le caractère déchiffré au StringBuilder.
                decrypted.Append((char)((c - k + 256) % 256));
            }

            // Retourne la chaîne déchiffrée.
            return decrypted.ToString();
        } // VigenereDecrypt

        /// <summary>
        /// Lit un mot de passe depuis la console sans afficher les caractères.
        /// </summary>
        /// <returns>Le mot de passe saisi par l'utilisateur.</returns>
        private string ReadPSW()
        {
            StringBuilder password = new StringBuilder();
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                // Ignorer toute touche qui n'est pas un caractère valide ou la touche Entrée.
                if (char.IsLetterOrDigit(key.KeyChar) || char.IsSymbol(key.KeyChar) || char.IsPunctuation(key.KeyChar))
                {
                    password.Append(key.KeyChar);
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b"); // Efface le dernier caractère de la console.
                }
            } while (key.Key != ConsoleKey.Enter);

            return password.ToString();
        }
    }
}
