namespace CasaXpsUtilities.Xps.Synchrotron.Diamond.Scans.IO;

using Ultimately;

/// <summary>
/// Provides a means for reading scan files.
/// </summary>
public interface IScanFileReader
{
    /// <summary>
    /// Reads a scan from the specified scan file.
    /// </summary>
    /// <param name="scanFile">The scan file to read from.</param>
    Option<Scan> Read(ScanFile scanFile);
    
    /// <summary>
    /// Reads a scan from a scan file located at the specified scan file path.
    /// </summary>
    /// <param name="filePath">The path to the file containing the scan data.</param>
    Option<Scan> Read(string filePath);
}
