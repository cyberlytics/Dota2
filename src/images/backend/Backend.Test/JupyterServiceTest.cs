﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Backend.Domain.Services;
using Backend.Domain.Models;


namespace Backend.Test
{
    [TestFixture]
    public class JupyterServiceTest
    {
        private MatchRepository _matchRepository;
        private MatchstoreDatabaseSettings _matchstoreDatabaseSettings;
        private OpenDotaCallerService _openDotaApi;
        private OpenDotaService _openDotaService;
        private JupyterService _jupyterService;

        // Sicher in den Matchverwaltungs- und -analyse-DBs hinterlegte Match-IDs
        private readonly long matchRepoExistingId = 6049286410;

        // Existierendes, nicht hinterlegtes Match
        private readonly long existingNewId = 6004345319;
        
        // Nicht in den DBs hinterlegte inkorrekte ID
        private readonly long wrongId = 123456;

        // Testvorbereitungen
        [SetUp]
        public async Task Setup()
        {
            _matchstoreDatabaseSettings = new MatchstoreDatabaseSettings()
            {
                MatchesCollectionName = "Matches",
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "MatchstoreDb"
            };
            _matchRepository = new MatchRepository(_matchstoreDatabaseSettings);
            
            _openDotaApi = new OpenDotaCallerService();

            _openDotaService = new OpenDotaService(_matchRepository, _openDotaApi);

            _jupyterService = new JupyterService(_matchRepository, _openDotaService);
            
            // Match in Analyse-DB anlegen
            // todo (jupyterExistingId durch ID eines bereits durch Seeder hinterlegtem Match ersetzen
            await _jupyterService.WriteMatchAsync(existingNewId);
            
            // Nur in Matchverwaltungs-DB existierendes Match aus Matchanalyse-DB loeschen
            await _jupyterService.DeleteMatchAsync(matchRepoExistingId);
        }

        /// <summary>
        /// Test, ob ein neu geholtes Zufallsmatch in die Matchanalyse-DB geschrieben werden kann
        /// </summary>
        [Test] public async Task WriteMatchAsync_Schreiben()
        {
            // Neues Match von OpenDota holen
            List<long> newMatchesIds = await _openDotaService.FetchNewMatches(1, true, true);
            long newMatchId = newMatchesIds.First();

            HttpStatusCode writeResult = await _jupyterService.WriteMatchAsync(newMatchId);

            // Wurde Match erfolgreich gespeichert?
            Assert.That((int) writeResult == 201);
            
            // Löschanfrage an Jupyter-Notebook-Server
            HttpStatusCode deleteResult = await _jupyterService.DeleteMatchAsync(matchRepoExistingId);
            
            // Wurde Match entfernt?
            Assert.That((int) deleteResult == 205);
        }

        /// <summary>
        /// Test, ob auf den Versuch, ein nicht existieredes
        /// Match in die Matchanalyse-DB zu schreiben, korrekt reagiert wird
        /// </summary>
        [Test]
        public async Task WriteMatchAsync_Falsches_Match_Schreiben()
        {
            // Match darf weder in der Datenbank verfuegbar noch ueber OpenDota abrufbar sein
            Assert.That(await _jupyterService.WriteMatchAsync(wrongId) == HttpStatusCode.BadRequest);
        }
        
        /// <summary>
        /// Test, ob auf den Versuch, ein bereits dort gespeichertes
        /// Match in die Matchanalyse-DB zu schreiben, korrekt reagiert wird
        /// </summary>
        [Test]
        public async Task WriteMatchAsync_Vorhandenes_Match_Schreiben()
        {
            Assert.That((int)await _jupyterService.WriteMatchAsync(existingNewId) == 409);
        }

        /// <summary>
        /// Test, ob ein existierendes Match erfolgreich aus der Matchanalyse-DB gelöscht werden kann
        /// </summary>
        [Test]
        public async Task DeleteMatchAsync_Match_Loeschen()
        {
            await _jupyterService.WriteMatchAsync(matchRepoExistingId);
            
            // Existierendes Match loeschen und wieder aufnehmen
            int resultCode = (int) await _jupyterService.DeleteMatchAsync(matchRepoExistingId);
            Console.Write(resultCode);            
            Assert.That(resultCode == 205);
            
            await _jupyterService.WriteMatchAsync(matchRepoExistingId);
        }
        
        /// <summary>
        /// Test, ob auf den Versuch, ein nicht existieredes
        /// Match aus der Matchanalyse-DB zu löschen, korrekt reagiert wird
        /// </summary>
        [Test]
        public async Task DeleteMatchAsync_Falsches_Match_Loeschen()
        {
            Assert.That((int)await _jupyterService.DeleteMatchAsync(wrongId) == 204);
        }

        /// <summary>
        /// Test, ob das Modell "no_kda" erfolgreich trainiert werden kann
        /// </summary>
        [Test]
        public async Task TrainModelAsync_Modell_No_Kda_Trainieren()
        {
            Assert.That((int) await _jupyterService.TrainModelAsync("no_kda") == 200);
        }
        
        /// <summary>
        /// Test, ob das Modell "kda" erfolgreich trainiert werden kann
        /// </summary>
        [Test]
        public async Task TrainModelAsync_Modell_Kda_Trainieren()
        {
            Assert.That((int) await _jupyterService.TrainModelAsync("kda") == 200);
        }

        /// <summary>
        /// Test, ob auf den Versuch, ein ungueltiges Modell zu trainieren, korrekt reagiert wird
        /// </summary>
        [Test]
        public async Task TrainModelAsync_Modell_Ungueltig_Trainieren()
        {
            Assert.That((int) await _jupyterService.TrainModelAsync("wrongmodel") == 400);
        }
        
        /// <summary>
        /// Test, ob auf den Versuch, ein Modell mit Leerstring als Namen zu trainieren, korrekt reagiert wird
        /// </summary>
        [Test]
        public async Task TrainModelAsync_Modell_Leer_Trainieren()
        {
            Assert.That((int) await _jupyterService.TrainModelAsync("") == 400);
        }
        
        /// <summary>
        /// Test, ob die Vorhersage mit dem Modell "no_kda" korrekt funktioniert
        /// </summary>
        [Test]
        public async Task Predict_Modell_No_Kda_Vorhersagen()
        {
            // Anfrage einer Vorhersageantwort
            PredictionResultDto result = await _jupyterService.PredictAsync(matchRepoExistingId, "no_kda");
            
            // Gueltiger Score?
            Assert.That(result.score > 0);
        }
        
        /// <summary>
        /// Test, ob die Vorhersage mit dem Modell "kda" korrekt funktioniert
        /// </summary>
        [Test]
        public async Task Predict_Modell_Kda_Vorhersagen()
        {
            // Anfrage einer Vorhersageantwort
            PredictionResultDto result = await _jupyterService.PredictAsync(matchRepoExistingId, "kda");
            
            // Gueltiger Score?
            Assert.That(result.score > 0);
        }
        
        /// <summary>
        /// Test, ob auf den Versuch, mit dem Modell "no_kda" ein ungueltiges Match vorherzusagen,
        /// korrekt reagiert wird
        /// </summary>
        [Test]
        public async Task Predict_Modell_No_Kda_Ungueltiges_Match_Vorhersagen()
        {
            Assert.That(await _jupyterService.PredictAsync(wrongId,"no_kda") == null);
        }
        
        /// <summary>
        /// Test, ob auf den Versuch, mit dem Modell "kda" ein ungueltiges Match vorherzusagen,
        /// korrekt reagiert wird
        /// </summary>
        [Test]
        public async Task Predict_Modell_Kda_Ungueltiges_Match_Vorhersagen()
        {
            Assert.That(await _jupyterService.PredictAsync(wrongId,"no_kda") == null);
        }
        
        /// <summary>
        /// Test, ob auf den Versuch, mit dem Modell "kda" ein neues Match vorherzusagen,
        /// korrekt reagiert wird
        /// </summary>
        [Test]
        public async Task Predict_Modell_No_Kda_Neues_Match_Vorhersagen()
        {
            // Anfrage einer Vorhersageantwort
            PredictionResultDto result;

            try
            {
                // Versuche, Vorhersage zu erhalten
                result = await _jupyterService.PredictAsync(matchRepoExistingId, "no_kda");
            
                // Gueltiger Score?
                Assert.That(result.score > 0);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }
        
        /// <summary>
        /// Test, ob auf den Versuch, mit dem Modell "kda" ein neues Match vorherzusagen,
        /// korrekt reagiert wird
        /// </summary>
        [Test]
        public async Task Modell_Kda_Neues_Match_Vorhersagen()
        {
            // Anfrage einer Vorhersageantwort
            PredictionResultDto result;

            try
            {
                // Versuche, Vorhersage zu erhalten
                result = await _jupyterService.PredictAsync(matchRepoExistingId, "kda");
            
                // Gueltiger Score?
                Assert.That(result.score > 0);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }
        
        /// <summary>
        /// Test, ob auf den Versuch, mit einem ungueltigen Modell ein Match vorherzusagen,
        /// korrekt reagiert wird
        /// </summary>
        [Test]
        public async Task Modell_Ungueltig_Vorhersagen()
        {
            Assert.That(await _jupyterService.PredictAsync(matchRepoExistingId,"wrongmodel") == null);
        }
        
        /// <summary>
        /// Test, ob auf den Versuch, mit einem ungueltigen Modell ein nicht existierendes Match vorherzusagen,
        /// korrekt reagiert wird
        /// </summary>
        [Test]
        public async Task Modell_Ungueltig_Ungueltiges_Match_Vorhersagen()
        {
            Assert.That(await _jupyterService.PredictAsync(wrongId,"wrongmodel") == null);
        }
    }
}