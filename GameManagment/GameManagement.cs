﻿using BatailleNavale;
using BatailleNavale.Gamer;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Text;
using System.Linq;
using BatailleNavale.GameManagment;

public class GameManagement
{
    public Gamer gamer;

    public Grid myGrid;

    public Grid adverseGrid;

    /// <summary>
    /// Place les navires
    /// </summary>
    public ShipPlacement shipPlacement;

    /// <summary>
    /// Nombre de bateaux du joueur
    /// </summary>
    public int ShipNumber = 1;
    public Point Blow;

    /// <summary>
    /// token est le jeton indiquant celui qui va commencer à jouer en premier lorsque si il est à True
    /// </summary>
    public bool token { get; set; } = true;


    public GameManagement()
    {
        gamer = new Gamer("Joueur1", "192.168.1.135");
        myGrid = new Grid(10);
        adverseGrid = new Grid(10);
        shipPlacement = new ShipPlacement(myGrid, gamer);
        Blow = new Point(-1, -1);
        DisplayGrid.Display(myGrid);


    }

    public void InitGame()
    {
        gamer.Ships = shipPlacement.ShipDeployment(ShipNumber);

    }

    public bool IsValideBlowEntred(int x, int y)
    {
        if (x < 0 || y < 0 || x > (adverseGrid.size - 1) || y > (adverseGrid.size - 1))
        {
            return false;
        }
        else
            return true;
    }

    public void StartGame()
    {

        while (true)
        {
            if (token)
            {
                Console.WriteLine("Saissisez le cooordonnées du COUP que vous souhaitez effectué ");
                string[] entredBlow = Console.ReadLine().ToUpper().Split(',');

                int entredBlowX = (int)Convert.ToChar(entredBlow[0][0]) - 'A';
                int entredBlowY = (int)Convert.ToChar(entredBlow[1][0]) - '0';

                Blow = new Point(entredBlowX, entredBlowY);

                while (!IsValideBlowEntred(Blow.X, Blow.Y))
                {
                    Console.WriteLine("Le COUP saisis est invalide!, Saissisez de nouveau :");
                    string[] newEntredBlow = Console.ReadLine().ToUpper().Split(',');

                    int newEntredBlowX = (int)Convert.ToChar(newEntredBlow[0][0]) - 'A';
                    int newEntredBlowY = (int)Convert.ToChar(newEntredBlow[1][0]) - '0';

                    Blow = new Point(newEntredBlowX, newEntredBlowY);
                }

                SendingBlow(Blow.X, Blow.Y);


                ReadReceivingMessageAndUpdateAdversairGrid();

                Console.WriteLine("My Grid");
                DisplayGrid.Display(myGrid);

                Console.WriteLine("Adversaire Grid");
                DisplayGrid.Display(adverseGrid);

                token = !token;
            }
            else
            {
                Console.WriteLine("Attente du COUP de l'adversaire ");
                var reveivedBlowPoint = ReceivingBlow();
                Console.WriteLine("Coup reçu! ");

                CheckReceivedBlow(reveivedBlowPoint);

                Console.WriteLine("My Grid");
                DisplayGrid.Display(myGrid);

                Console.WriteLine("Adversaire Grid");
                DisplayGrid.Display(adverseGrid);


                token = !token;

            }
        }

    }


    public void GiveFeedbackToAdverser(string message)
    {
        Communication.SendMessage(message, gamer.IPAddress);
    }
    public void CheckReceivedBlow(Point p)
    {
        if (myGrid.matrix[p.X][p.Y].IsOccupied)
        {
            var state = "Touche";
            myGrid.matrix[p.X][p.Y].IsTouched = true;
            myGrid.matrix[p.X][p.Y].CellType = CellType.CELL_ISTOUCHED; ;
            Console.WriteLine("CheckReceivedBlow");
            DisplayGrid.Display(myGrid);

            var releventShip = (from ship in gamer.Ships
                                from position in ship.Position
                                where position == p
                                select ship).ToList().First();
            releventShip.LifePoint--;
            if (releventShip.LifePoint == 0)
            {
                Console.WriteLine(releventShip.Name + "dont l'ID est : {" + releventShip.ID + "} est coulé");
                state = "Coule";
                releventShip.ShipState = ShipState.ShipBlowed;
            }
            GiveFeedbackToAdverser(state);
        }
        else
        {
            myGrid.matrix[p.X][p.Y].IsMisHit = true;
            myGrid.matrix[p.X][p.Y].CellType = CellType.CELL_MISHIT;

            GiveFeedbackToAdverser("Rate");

        }
    }

    public void ReadReceivingMessageAndUpdateAdversairGrid()
    {
        string message = Communication.ReceiveMessage();

        if (message == "Touche")
        {
            adverseGrid.matrix[Blow.X][Blow.Y].IsTouched = true;
            adverseGrid.matrix[Blow.X][Blow.Y].CellType = CellType.CELL_ISTOUCHED;
        }

        if (message == "Rate")
        {
            adverseGrid.matrix[Blow.X][Blow.Y].IsMisHit = true;
            adverseGrid.matrix[Blow.X][Blow.Y].CellType = CellType.CELL_MISHIT;

        }

        if (message == "Coule")
        {
            adverseGrid.matrix[Blow.X][Blow.Y].IsTouched = true;
            adverseGrid.matrix[Blow.X][Blow.Y].CellType = CellType.CELL_ISTOUCHED;

            Console.WriteLine("Le Bateaux est coulé");

        }
    }
    /// <summary>
    /// Envoie les coups d'un joueur à un autre
    /// </summary>
    /// <param name="token">Indique si le joueur à le droit d'agir</param>
    /// <param name="x">Abscisse du point d'arrivée</param>
    /// <param name="y">Ordonnée du point d'arrivée</param>
    /// 
    public void SendingBlow(int x, int y)
    {
        Communication.SendMessage(x.ToString() + "," + y.ToString(), gamer.IPAddress);
    }

    /// <summary>
    /// Vérifie si un joueur peut recevoir des coups
    /// </summary>
    /// <param name="token">Indique si le joueur à le droit d'agir</param>
    public Point ReceivingBlow()
    {

        string message = Communication.ReceiveMessage();
        string[] msgSplited = message.Split(',');

        return new Point(Convert.ToInt32(msgSplited[0]), Convert.ToInt32(msgSplited[1]));
    }

    /// <summary>
    /// Renvoie le contenu d'une cellule de coordonnées p
    /// </summary>
    /// <param name="p">Coordonnées de la céllule recherchée</param>
    /// <returns>Valeur de la cellule recherchée</returns>
    public Cell CellContent(Point p)
    {
        return (from list in myGrid.matrix
                from cell in list
                where cell.PointCoordinate == p
                select cell).ToList().First();
    }


}





