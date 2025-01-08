string? filePath = args.FirstOrDefault();

if (filePath == null) {
    Console.WriteLine("Drag and drop desired .rbxl file into exe.");
    Console.ReadLine();
    return;
}

var info = new FileInfo(filePath);
if (info.Extension != ".rbxl") {
    return;
}

using (FileStream stream = File.OpenRead(filePath)) {
    BinaryReader reader = new(stream);
}

Console.ReadLine();