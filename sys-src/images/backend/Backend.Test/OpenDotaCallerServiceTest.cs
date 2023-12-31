﻿using System;
using System.Net;
using NUnit.Framework;
using Backend.Domain.Services;
using System.Net.Http;
using System.Threading.Tasks;

namespace Backend.Test
{
    [TestFixture]
    public class OpenDotaCallerServiceTest
    {
        private OpenDotaCallerService _openDotaCallerService;
        
        // Testvorbereitungen
        [SetUp]
        public void Setup()
        {
            _openDotaCallerService = new OpenDotaCallerService();
        }

        /// <summary>
        /// Test, ob das Abfragen von Matches ueber OpenDota funktioniert
        /// </summary>
        [Test]
        public async Task GetValue_Matches_Abfragen()
        {
            // Public Matches anfragen (valide Anfrage)
            var getValue = await _openDotaCallerService.GetValue("https://api.opendota.com/api/publicMatches");

            Assert.That(getValue.Length > 0);
        }
        
        /// <summary>
        /// Test, ob das inkorrekte Abfragen von OpenDota entsprechend reagiert
        /// </summary>
        [Test]
        public async Task GetValue_Unvollstaendige_Url_Abrufen()
        {
            // Valide URL ausserhalb der Schnittstelle anfragen
            var getValue = await _openDotaCallerService.GetValue("https://api.opendota.com");
            
            Assert.That(getValue.Length > 0);
        }
        
        /// <summary>
        /// Test, ob das Abfragen inkorrekter URLs entsprechend reagiert
        /// </summary>
        [Test]
        public async Task GetValue_Falsche_Url_Abrufen ()
        {
            
            // Invalide URL, Fehler muss geworfen werden
            string getValue = null;
            try
            {
                getValue = await _openDotaCallerService.GetValue("WRONG");
            }
            catch (Exception e)
            {
            }

            Assert.That(getValue == null);
        }
        
        /// <summary>
        /// Test, ob Post-Anfragen fuer Parseanfragen an OpenDota korrekt funktionieren
        /// </summary>
        [Test]
        public async Task PostValue_Match_Anfragen()
        {
            // Parseanfrage fuer existierendes Match
            var postValue = await _openDotaCallerService.PostValue("https://api.opendota.com/api/request/6049286410",new StringContent(""));
            Assert.That(postValue.StatusCode == HttpStatusCode.OK);
        }
        
        /// <summary>
        /// Test, ob fehlerhafte Parseanfragen an OpenDota entsprechend reagieren
        /// </summary>
        [Test]
        public async Task PostValue_Fehlerhaft_Anfragen()
        {
            // Invalide Parseanfrage
            var postValue = await _openDotaCallerService.PostValue("https://api.opendota.com/api/request",new StringContent(""));
            Assert.That(postValue.StatusCode == HttpStatusCode.NotFound);
        }
        
        /// <summary>
        /// Test, ob Parseanfragen an OpenDota fuer nicht existierende Matches entsprechend reagieren
        /// </summary>
        [Test]
        public async Task PostValue_Nicht_Existentes_Match_Anfragen()
        {
            // Invalide Parseanfrage
            var postValue = await _openDotaCallerService.PostValue("https://api.opendota.com/api/request/1234567890",new StringContent(""));
            Console.Write(postValue.StatusCode);
            Assert.That(postValue.StatusCode == HttpStatusCode.OK);
        }
    }
}