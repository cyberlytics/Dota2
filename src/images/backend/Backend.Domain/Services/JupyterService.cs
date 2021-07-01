using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Backend.Domain.Models;
using Newtonsoft.Json;

namespace Backend.Domain.Services
{
    public class JupyterService : IJupyterService
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IOpenDotaService _openDotaService;
        private HttpClient _httpClient;
        private static string pythonUrl = "http://host.docker.internal:8898";

        public JupyterService(IMatchRepository matchRepository, IOpenDotaService openDotaService)
        {
            _matchRepository = matchRepository;
            _openDotaService = openDotaService;
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Service zum Schreiben eines Matches anhand einer MatchID auf dem Jupyter-Server.
        /// </summary>
        /// <param name="matchId">MatchId des zu schreibenden Matches</param>
        /// <returns>HttpStatusCode, 200 wenn das Spiel gespeichert werden konnte, 400, wenn eine ungültige matchId übergeben wurde, 500, wenn ein Fehler aufgetreten ist.</returns>
        public async Task<HttpStatusCode> WriteMatchAsync(long matchId)
        {
            var url = $"{pythonUrl}/writematch";

            //Wenn kein Match in der Datenbank gefunden wurde, hole und parse das Match
            var match = _matchRepository.Get(matchId);
            if (match == null)
            {
                match = await _openDotaService.FetchMatch(matchId);
            }

            string matchDtoJson;
            try
            {
                //Versuche Match in MatchDto zu konvertieren und als JSON String zu serialisieren
                var matchDto = new MatchDto(match);
                matchDtoJson = JsonConvert.SerializeObject(matchDto);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return HttpStatusCode.InternalServerError;
            }
            
            try
            {
                //Versuche, JSON String an Server zu senden.
                var request = new HttpRequestMessage(new HttpMethod("POST"), url);
                request.Content = new StringContent(matchDtoJson);
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                var responseMessage = await _httpClient.SendAsync(request);
                
                return responseMessage.StatusCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return HttpStatusCode.InternalServerError;
            }
        }

        /// <summary>
        /// Service zum Löschen eines Matches anhand einer MatchID auf dem Jupyter-Server.
        /// </summary>
        /// <param name="matchId">MatchId des zu schreibenden Matches</param>
        /// <returns>HttpStatusCode, 200 wenn das Spiel gelöscht werden konnte, 400, wenn eine ungültige matchId übergeben wurde, 500, wenn ein Fehler aufgetreten ist.</returns>
        public async Task<HttpStatusCode> DeleteMatchAsync(long matchId)
        {
            var url = $"{pythonUrl}/deletematch";
            
            try
            {
                //Versuche, ID zum Löschen an Server zu senden.
                var request = new HttpRequestMessage(new HttpMethod("POST"), url);
                request.Content = new StringContent($"{{{matchId}}}");
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                var responseMessage = await _httpClient.SendAsync(request);
                
                return responseMessage.StatusCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return HttpStatusCode.InternalServerError;
            }
        }

        /// <summary>
        /// Service zum Starten des Trainings eines Models.
        /// </summary>
        /// <param name="model_name">Welches Model verwendet werden soll. "no_kda" oder "kda"</param>
        /// <returns>StatusCode des Jupyter Servers</returns>
        public async Task<HttpStatusCode> TrainModelAsync(string model_name)
        {
            var url = $"{pythonUrl}/trainmodel";

            try
            {
                //Versuche, JSON String an Server zu senden.
                var request = new HttpRequestMessage(new HttpMethod("POST"), url);
                request.Content = new StringContent(model_name);
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                var responseMessage = await _httpClient.SendAsync(request);

                return responseMessage.StatusCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return HttpStatusCode.InternalServerError;;
            }
        }
        
        /// <summary>
        /// Service zum Erstellen einer Prediction.
        /// </summary>
        /// <param name="matchId">ID des zu vorhersagenden Matches.</param>
        /// <param name="model_name">Welches Model verwendet werden soll. "no_kda" oder "kda"</param>
        /// <returns>Liste der PredictionResults.</returns>
        public async Task<PredictionResultDto> PredictAsync(long matchId, string model_name)
        {
            var url = $"{pythonUrl}/predict";

            //Wenn kein Match in der Datenbank gefunden wurde, hole und parse das Match
            var match = _matchRepository.Get(matchId);
            if (match == null)
            {
                match = await _openDotaService.FetchMatch(matchId);

                if (match == null)
                {
                    //Match entspricht nicht den Anforderungen
                    return null;
                }
            }

            var matchDto = new MatchDto(match);
            var predictionDto = new PredictionDto(model_name, new List<MatchDto>(){matchDto});
            
            string predictionDtoJson;
            try
            {
                //Versuche PredictionDto in JSON String zu serialisieren
                predictionDtoJson = JsonConvert.SerializeObject(predictionDto);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
            
            try
            {
                //Versuche, JSON String an Server zu senden.
                var request = new HttpRequestMessage(new HttpMethod("POST"), url);
                request.Content = new StringContent(predictionDtoJson);
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                var responseMessage = await _httpClient.SendAsync(request);
                var responseJson = await responseMessage.Content.ReadAsStringAsync();
                var predictionResultDtos = JsonConvert.DeserializeObject<PredictionResultDto>(responseJson, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                
                return predictionResultDtos;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}