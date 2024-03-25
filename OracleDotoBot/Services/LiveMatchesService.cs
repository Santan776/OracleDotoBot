﻿using OracleDotoBot.Abstractions.Services;
using OracleDotoBot.Domain.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace OracleDotoBot.Services
{
    public class LiveMatchesService : ILiveMatchesService
    {
        public LiveMatchesService(ISteamApiService steamApiService, 
            IMatchesResultService matchesResultService)
        {
            _steamApiService = steamApiService;
            _matchesResultService = matchesResultService;
            LiveMatches = new List<(Match match, string analitics)>();
        }

        public List<(Match match, string analitics)> LiveMatches {  get; private set; }

        private readonly ISteamApiService _steamApiService;
        private readonly IMatchesResultService _matchesResultService;

        public async Task UpdateLiveMatches()
        {
            var matches = await _steamApiService.GetLiveMatches();

            var liveMatches = new List<(Match match, string analitics)>();

            foreach (var m in matches)
            {
                var analitics = await _matchesResultService.GetMatchResult(m, false, true);
                liveMatches.Add((m, analitics));
            }

            LiveMatches = liveMatches;
        }

        public ReplyKeyboardMarkup GetLiveMatchesKeyboard()
        {
            var keyboard = new ReplyKeyboardMarkup(
                new List<KeyboardButton[]>()
                {
                new KeyboardButton[]
                {
                    new KeyboardButton(LiveMatches.Count > 0 ?
                        $"{ LiveMatches[0].match.RadiantTeam.Name } VS { LiveMatches[0].match.DireTeam.Name }" : "no match"),
                    new KeyboardButton(LiveMatches.Count > 1 ?
                        $"{ LiveMatches[1].match.RadiantTeam.Name } VS { LiveMatches[1].match.DireTeam.Name }" : "no match")
                },
                new KeyboardButton[]
                {
                    new KeyboardButton(LiveMatches.Count > 2 ?
                        $"{LiveMatches[2].match.RadiantTeam.Name} VS {LiveMatches[2].match.DireTeam.Name}" : "no match"),
                    new KeyboardButton(LiveMatches.Count > 3 ?
                        $"{LiveMatches[3].match.RadiantTeam.Name} VS {LiveMatches[3].match.DireTeam.Name}" : "no match")
                },
                new KeyboardButton[]
                {       
                    new KeyboardButton(LiveMatches.Count > 4 ?
                        $"{ LiveMatches[4].match.RadiantTeam.Name } VS { LiveMatches[4].match.DireTeam.Name }" : "no match"),
                    new KeyboardButton(LiveMatches.Count > 5 ?
                        $"{ LiveMatches[5].match.RadiantTeam.Name } VS { LiveMatches[5].match.DireTeam.Name }" : "no match")
                },
                new KeyboardButton[]
                {
                    new KeyboardButton(LiveMatches.Count > 6 ?
                        $"{LiveMatches[6].match.RadiantTeam.Name} VS {LiveMatches[6].match.DireTeam.Name}" : "no match"),
                    new KeyboardButton("Назад")
                }
                })
            {
                ResizeKeyboard = true,
            };

            return keyboard;
        }
    }
}
