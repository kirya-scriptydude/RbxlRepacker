using System.Text;
using K4os.Compression.LZ4;
using ZstdSharp;

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

ulong fileSize = 32;
using (FileStream stream = File.OpenRead(filePath)) {
    BinaryReader reader = new(stream);

    //File header
    writer.Write(reader.ReadBytes(14));
    writer.Write(reader.ReadUInt16()); //version
    writer.Write(reader.ReadInt32()); //inst chunks
    writer.Write(reader.ReadInt32()); //classes
    writer.Write(reader.ReadInt64()); //reserved

    bool endReached = false;
    while (!endReached) {

        byte[] chunkName = reader.ReadBytes(4);
        int compSize = reader.ReadInt32();
        int size = reader.ReadInt32();
        reader.ReadInt32(); //reserved
        fileSize += 16;

        bool isCompressed = compSize > 0;

        byte[] data;

        if (!isCompressed) {
            data = reader.ReadBytes(size);
        } else {
            byte[] compressedData = reader.ReadBytes(compSize);

            if (BitConverter.ToString(compressedData, 1, 3) == "B5-2F-FD") {
                //zstd
                data = new byte[size];
                var decomp = new Decompressor();
                decomp.Unwrap(compressedData, 0, compressedData.Length, data, 0, data.Length);
            } else {
                //lz4
                data = new byte[size];
                LZ4Codec.Decode(compressedData, 0, compressedData.Length, data, 0, data.Length);
            }
        }

        writer.Write(chunkName);
        writer.Write(size);
        writer.Write(0); //compressed size
        writer.Write(0); //reserved
        writer.Write(data); //data

        fileSize += (ulong)data.Length;

        string chunkNameString = Encoding.ASCII.GetString(chunkName);

        if (chunkNameString == "END\0") 
            endReached = true;
        
        Console.WriteLine($"Writing {chunkNameString}: {size} bytes... (total: {fileSize})");
    }
}

Console.WriteLine($"{fileStr.Name} finished decompresing. Size = {fileSize / 1000} KB");

fileStr.Dispose();
Console.ReadLine();
