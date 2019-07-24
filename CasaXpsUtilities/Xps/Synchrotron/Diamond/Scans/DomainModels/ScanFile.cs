namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans.DomainModels
{
    using Ultimately;
    using Ultimately.Utilities;

    using System.IO;
    using System.Text.RegularExpressions;


    public class ScanFile
    {
        internal static Regex MatchScanNumber => new Regex(@"^i09-(\d+)");


        public string Filepath { get; }

        public string Filename { get; }

        public uint Number { get; }


        private ScanFile(string filepath, string filename, uint number)
        {
            Filepath = filepath;
            Filename = filename;
            Number = number;
        }


   
        public static Option<ScanFile> Create(string filepath, uint number)
        {
            return NotEmptyFilepath(filepath).Map(() => new ScanFile(filepath, Path.GetFileName(filepath), number));
        }

        public static Option<ScanFile> Create(string filepath)
        {
            return NotEmptyFilepath(filepath).Map(() => Path.GetFileName(filepath))
                                             .FlatMap(fn => MatchScanNumber.Match(fn).SomeWhen(m => m.Success, $"Could not parse scan number from filename '{fn}'")
                                                                                     .Map(m => new
                                                                                         {
                                                                                             Filename = fn,
                                                                                             Match = m
                                                                                         }))
                                             .FlatMap(m => TryParse.ToUInt(m.Match.Groups[1].Value, $"Parsed scan number too big. Filename: '{m.Filename}'").Map(sn => new
                                                                                                                                                                {
                                                                                                                                                                    m.Filename,
                                                                                                                                                                    Number = sn
                                                                                                                                                                }))
                                             .Map(sf => new ScanFile(filepath, sf.Filename, sf.Number));
        }



        private static Option NotEmptyFilepath(string filepath)
        {
            return Optional.SomeWhen(!string.IsNullOrWhiteSpace(filepath), "Filepath to scan cannot be empty");
        }



        public override string ToString() => Filename;
    }
}
