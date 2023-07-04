using Newtonsoft.Json;
using Questao2.Models;
using System.Net;

public class Program
{


    public static void Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static int getTotalScoredGoals(string team, int year)
    {
        string apiUrl = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&team1={team}";


        HttpClient client = new HttpClient();
        HttpResponseMessage response = client.GetAsync(apiUrl).Result;
        response.EnsureSuccessStatusCode();

        string responseBody = response.Content.ReadAsStringAsync().Result;
        return CalculateTotalGoals(responseBody, apiUrl);

    }

    private static int CalculateTotalGoals(string responseBody, string apiUrl)
    {
        int totalGoals  = 0;
        string urlTeam2 = apiUrl.Replace("team1", "team2");
        totalGoals += GetTotalTeam1Goals(apiUrl);
        totalGoals += GetTotalTeam2Goals(urlTeam2);

        return totalGoals;
    }


    public static int GetTotalTeam1Goals(string apiUrl)
    {
        int totalGoals = 0;
        int page = 1;
        int totalPages = 1;

        while (page <= totalPages)
        {
            string url = $"{apiUrl}&page={page}";
            string json = GetJsonFromUrl(url);
            FootballMatches apiResponse = JsonConvert.DeserializeObject<FootballMatches>(json);

            totalPages = apiResponse.total_pages;

            foreach (MatchData matchData in apiResponse.data)
            {
                int goals;
                if (int.TryParse(matchData.team1goals, out goals))
                {
                    totalGoals += goals;
                }
            }

            page++;
        }

        return totalGoals;
    }

    public static int GetTotalTeam2Goals(string apiUrl)
    {
        int totalGoals = 0;
        int page = 1;
        int totalPages = 1;

        while (page <= totalPages)
        {
            string url = $"{apiUrl}&page={page}";
            string json = GetJsonFromUrl(url);
            FootballMatches apiResponse = JsonConvert.DeserializeObject<FootballMatches>(json);

            totalPages = apiResponse.total_pages;

            foreach (MatchData matchData in apiResponse.data)
            {
                int goals;
                if (int.TryParse(matchData.team2goals, out goals))
                {
                    totalGoals += goals;
                }
            }

            page++;
        }

        return totalGoals;
    }

    public static string GetJsonFromUrl(string url)
    {
        using (WebClient client = new WebClient())
        {
            return client.DownloadString(url);
        }
    }



}