using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}

public sealed class Constants
{
    public class Prefs {
        public static string currentID = "currentID";
        public static string currentProof = "currentProof";
        public static string proofExpireDate = "proofExpireDate";
        public static string masterVolume = "masterVolume";
        public static string musicCapVolume = "musicCapVolume";
        public static string musicFade = "musicFadeVolume";
        public static string sfxCapVolume = "sfxCapVolume";
        public static string screenModeKey = "screenModeKey";
        public static string selectedTheme = "selectedTheme";
        public static string reMatch = "reMatch";
    }

    public class GameSettings{
        public static float transitionAnimationTime = 0.5f;
        public static int attackSkipID = -1;
        public static int defenseSkipID = -2;
        public static float opponentPlayCardDelay = 2.0f;
        public static float playerPlayCardDelay = 1f;
    }

    public class Colors
    {
        public static Color idleUiColor = new Color(1f, 0f, 0f, 1f);
        public static Color selectedUiColor = new Color(0.55f, 1f, 0.4f, 1f);
    }

    public class Tips
    {
        public static List<string> tips = new List<string>()
        {
            "Patients can be asymptomatic.",
            "You can skip the first chapter by sending the patient straight home.",
            "Patients can only be infected by one pathogen at a time."
        };
    }

    public class Patients
    {
        public static List<string> maleNames = new List<string>()
        {
            "Liam Johnson",
            "Noah Brown",
            "Mason Garcia",
            "Ethan Davis",
            "Lucas Martinez",
            "Jackson Lopez",
            "Aiden Wilson",
            "Logan Thomas",
            "Benjamin Moore",
            "James Martin",
            "Sebastian Perez",
            "Elijah White",
            "Alexander Sanchez",
            "Jacob Ramirez",
            "Henry Walker",
            "Daniel King",
            "Matthew Scott",
            "David Adams",
            "Joseph Nelson",
            "Riley Carter",
            "Samuel Mitchell",
            "Andrew Roberts",
            "Joshua Turner",
            "Gabriel Evans",
            "William Collins",
            "Michael Morris",
            "Christopher Rivera",
            "Ryan Rogers",
            "John Reed",
            "Owen Cooper",
            "Nathan Bailey",
            "Caleb Howard",
            "Dylan Torres",
            "Luke Gray",
            "Adam James",
            "Isaac Brooks",
            "Julian Sanders",
            "Grayson Foster",
            "Levi Long",
            "Hunter Patterson",
            "Christian Jenkins",
            "Jonathan Myers",
            "Isaiah Boyd",
            "Charles Kelly",
            "Thomas Ross",
            "Josiah Hamilton",
            "Hudson Griffin"
        };
        public static List<string> femaleNames = new List<string>()
        {
            "Ava Smith",
            "Emma Williams",
            "Olivia Jones",
            "Sophia Miller",
            "Isabella Rodriguez",
            "Mia Hernandez",
            "Amelia Gonzalez",
            "Harper Anderson",
            "Charlotte Taylor",
            "Ella Jackson",
            "Abigail Lee",
            "Avery Thompson",
            "Scarlett Harris",
            "Grace Clark",
            "Chloe Lewis",
            "Evelyn Robinson",
            "Aria Young",
            "Layla Wright",
            "Lily Green",
            "Ellie Baker",
            "Zoe Campbell",
            "Nora Phillips",
            "Hannah Parker",
            "Lillian Edwards",
            "Hazel Stewart",
            "Eleanor Murphy",
            "Penelope Cook",
            "Sophie Morgan",
            "Aubrey Bell",
            "Victoria Richardson",
            "Madison Cox",
            "Natalie Ward",
            "Camila Peterson",
            "Aurora Ramirez",
            "Brooklyn Washington",
            "Paisley Bennett",
            "Savannah Perry",
            "Stella Powell",
            "Allison Russell",
            "Violet Griffin",
            "Alice Webb",
            "Clara Haynes",
            "Bella Alexander",
            "Samantha Gonzales",
            "Lucy Bryant",
            "Audrey Russell"
        };
        public static List<string> Professions = new List<string>()
        {
            "Doctor",
            "Engineer",
            "Teacher",
            "Scientist",
            "Artist",
            "Nurse",
            "Police Officer",
            "Firefighter",
            "Pilot",
            "Chef",
            "Musician",
            "Writer",
            "Lawyer",
            "Architect",
            "Pharmacist"
        };
    }
}
