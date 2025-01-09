string? filePath = args.FirstOrDefault();

#if DEBUG
filePath = @"C:\Users\sbox\Documents\Baseplate.rbxl";
#endif

if (filePath == null) {
    Console.WriteLine("Drag and drop desired .rbxl file into exe.");
    Console.ReadLine();
    return;
}

var info = new FileInfo(filePath);
if (info.Extension != ".rbxl") {
    return;
}

FileStream fileStr = File.Create($"{info.Name}.decompressed.rbxl");
BinaryWriter writer = new(fileStr);

using (FileStream stream = File.OpenRead(filePath)) {
    BinaryReader reader = new(stream);
    //File header
    writer.Write(reader.ReadBytes(8)); //magic number
    writer.Write(reader.ReadBytes(6)); //signature
    writer.Write(reader.ReadUInt16()); //version
    writer.Write(reader.ReadInt32()); //inst chunks
    writer.Write(reader.ReadInt32()); //classes
    writer.Write(reader.ReadBytes(8)); //reserved

}

fileStr.Dispose();
Console.ReadLine();