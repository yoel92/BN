﻿//using BatailleNavale;
//using System.Text.Json;
using BatailleNavale;
using BatailleNavale.GameManagment;
using BatailleNavale.Gamer;
using BatailleNavale;
using System.Drawing;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

class Program
{
    static void Main(String[]? args)
    {
        Console.WriteLine("Ceci est un jeu de bataille navale.");
        TestBateaux();
        //TestGrille();
    }

    static void TestBateaux()
    {
        // Affichage des types de bateaux
        List<ShipType>? ListeTypes = new List<ShipType>();
        String? jsonString = File.ReadAllText("typesBateaux.json");
        ListeTypes = JsonSerializer.Deserialize<List<ShipType>>(jsonString, new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Latin1Supplement),
            WriteIndented = true
        });

        foreach (ShipType tb in ListeTypes) Console.WriteLine($"Catégorie : {tb.ModelName} - Taille : {tb.Size}");
        Console.WriteLine();

        // Affichage des bateaux
        List<Ship> ListeBateaux = new List<Ship>();
        Random rnd = new Random();
        for (int i = 0; i < 10; i++) ListeBateaux.Add(new Ship(ListeTypes[rnd.Next(ListeTypes.Count)], new Point(rnd.Next(0, 8), rnd.Next(0, 8))));

        foreach (Ship b in ListeBateaux) Console.WriteLine($"{b.Name} ({b.ID}) - Taille {b.Size} - Solidité {b.LifePoint}/{b.Size} - Position ({b.ShipStartPointCoordinate.X}, {b.ShipStartPointCoordinate.Y}) ({Ship.DescriptionEtatBateau[(int)b.ShipState]})");
        Console.WriteLine();

        // Attaques
        for (int i = 0; i < 20; i++)
        {
            int index = rnd.Next(ListeBateaux.Count);
            ListeBateaux[index].EstAttaque();
            Console.WriteLine($"{ListeBateaux[index].Name} ({ListeBateaux[index].ID}) - Taille {ListeBateaux[index].Size} - Solidité {ListeBateaux[index].LifePoint}/{ListeBateaux[index].Size} - Position ({ListeBateaux[index].ShipStartPointCoordinate.X}, {ListeBateaux[index].ShipStartPointCoordinate.Y}) ({Ship.DescriptionEtatBateau[(int)ListeBateaux[index].ShipState]})");
        }
    }

    static void TestGrille()
    {
        Console.WriteLine("Création du tableau :");
        String? jsonString = File.ReadAllText("typesBateaux.json");
        List<ShipType>? ListeTypes = JsonSerializer.Deserialize<List<ShipType>>(jsonString);
        Grid Grid = new Grid(9);
        Console.WriteLine("\nEtat des cases :");
        Grid.Show();

        Console.WriteLine("Fin de TestGrille.");
    }
}

GameManagement gridManagment = new GameManagement();

gridManagment.InitGame();




using BatailleNavale;

Grid grid1 = new Grid();
grid1.InitGrid(10);

*/