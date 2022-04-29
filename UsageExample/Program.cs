// See https://aka.ms/new-console-template for more information

using BG.WkHtmlToImage.Net;

Console.WriteLine("Starting...");
Random random = new Random();
int wordId = random.Next(1, 100);

// bool result = new HtmlToImageConverter().Convert(
//     $"https://enapi.wordwordapp.com/guessword?id={wordId}&shareType=0", 
//     $"/Users/da/Downloads/{wordId}.jpg",
//     "--width 1024 --height 1024 --quality 75");

string result = new HtmlToImageConverter().Convert(
    args[0], 
    args[1],
    "--width 1024 --height 1024 --quality 75");

Console.WriteLine($"[Success]: {result}");