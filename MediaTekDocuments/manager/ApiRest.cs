﻿using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;

namespace MediaTekDocuments.manager
{
    /// <summary>
    /// Classe indépendante d'accès à une api rest avec éventuellement une "basic authorization"
    /// </summary>
    class ApiRest
    {
        /// <summary>
        /// unique instance de la classe
        /// </summary>
        private static ApiRest instance = null;
        /// <summary>
        /// Objet de connexion à l'api
        /// </summary>
        private readonly HttpClient httpClient;
        /// <summary>
        /// Canal http pour l'envoi du message et la récupération de la réponse
        /// </summary>
        private HttpResponseMessage httpResponse;

        /// <summary>
        /// Constructeur privé pour préparer la connexion (éventuellement sécurisée)
        /// </summary>
        /// <param name="uriApi">adresse de l'api</param>
        /// <param name="authenticationString">chaîne d'authentification</param>
        private ApiRest(String uriApi, String authenticationString="")
        {
            httpClient = new HttpClient() { BaseAddress = new Uri(uriApi) };
            // prise en compte dans l'url de l'authentificaiton (basic authorization), si elle n'est pas vide
            if (!String.IsNullOrEmpty(authenticationString))
            {
                String base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + base64EncodedAuthenticationString);
            }
        }

        /// <summary>
        /// Crée une instance unique de la classe
        /// </summary>
        /// <param name="uriApi">adresse de l'api</param>
        /// <param name="authenticationString">chaîne d'authentificatio (login:pwd)</param>
        /// <returns></returns>
        public static ApiRest GetInstance(String uriApi, String authenticationString)
        {
            if(instance == null)
            {
                instance = new ApiRest(uriApi, authenticationString);
            }
            return instance;
        }

        /// <summary>
        /// Envoi une demande à l'API et récupère la réponse
        /// </summary>
        /// <param name="methode">verbe http (GET, POST, PUT, DELETE)</param>
        /// <param name="message">message à envoyer dans l'URL</param>
        /// <param name="parametres">contenu de variables à mettre dans body</param>
        /// <returns>liste d'objets (select) ou liste vide (ok) ou null si erreur</returns>

        public JObject RecupDistant(string methode, string message, string parametres)
        {
            // Avant d'envoyer la requête
            string fullUrl = httpClient.BaseAddress + message;
            string bodyContent = parametres ?? "";

            // Décode le paramètre pour affichage propre
            string decodedParams = Uri.UnescapeDataString(bodyContent);

            Console.WriteLine("🔽 Requête envoyée à l'API 🔽");
            Console.WriteLine($"➡️ Méthode : {methode}");
            Console.WriteLine($"➡️ URL     : {fullUrl}");
            Console.WriteLine($"➡️ Body (encodé)   : {bodyContent}");
            Console.WriteLine($"➡️ Body (décodé)   : {decodedParams}\n");


            StringContent content = null;
            if (!string.IsNullOrEmpty(parametres))
            {
                content = new StringContent(parametres, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
            }

            switch (methode)
            {
                case "GET":
                    httpResponse = httpClient.GetAsync(message).Result;
                    break;
                case "POST":
                    httpResponse = httpClient.PostAsync(message, content).Result;
                    break;
                case "PUT":
                    httpResponse = httpClient.PutAsync(message, content).Result;
                    break;
                case "DELETE":
                    if (content != null)
                    {
                        // Crée une requête DELETE avec body
                        var request = new HttpRequestMessage(HttpMethod.Delete, message)
                        {
                            Content = content
                        };
                        httpResponse = httpClient.SendAsync(request).Result;
                    }
                    else
                    {
                        httpResponse = httpClient.DeleteAsync(message).Result;
                    }
                    break;

                default:
                    return new JObject();
            }

            string responseData = httpResponse.Content.ReadAsStringAsync().Result;

            try
            {
                return JObject.Parse(responseData);
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Erreur JSON : " + ex.Message + "\nRéponse brute :\n" + responseData);
                return new JObject(); // ✅ Correction SonarLint S1168
            }
        }



    }
}
