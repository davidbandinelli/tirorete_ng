using PBEMFootball.Common.Models;

Console.WriteLine("=== PBEM Football Client ===");
Console.WriteLine("Benvenuto nel client del gioco PBEM Football.");
Console.WriteLine();

while (true) {
    Console.WriteLine("Menu principale:");
    Console.WriteLine("1. Visualizza squadra");
    Console.WriteLine("2. Allena giocatori");
    Console.WriteLine("3. Prepara formazione");
    Console.WriteLine("4. Esci");
    Console.Write("Scegli un'opzione: ");
    var choice = Console.ReadLine();
    if (choice == "4") break;
    Console.WriteLine("Opzione selezionata: " + choice);
}