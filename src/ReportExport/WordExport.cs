using System;
using System.IO;

namespace accounts.ReportExport
{
    static class WordExport
    {
        public static string CreateXPS(string source)
        {
            string xpsfn = null;
            try
            {
                if (source != null && File.Exists(source))
                {
                    Spire.Doc.Document doc = new Spire.Doc.Document();
                    doc.LoadFromFile(source);
                    xpsfn = public_members.GnenerateFileName(public_members.reportPath + @"\BookKeeper\xps", ".xps"); 
                    doc.SaveToFile(xpsfn, Spire.Doc.FileFormat.XPS);
                    doc.Close();
                }
            }
            catch (Exception)
            {

                throw;
            }
            return xpsfn;
        }
        public static string CreatePDF(string source)
        {
            string fdffn = null;
            try
            {
                if (source != null && File.Exists(source))
                {
                    Spire.Doc.Document doc = new Spire.Doc.Document();
                    doc.LoadFromFile(source);
                    fdffn = public_members.GnenerateFileName(public_members.reportPath + @"\BookKeeper\pdf", ".pdf");
                    doc.SaveToFile(fdffn, Spire.Doc.FileFormat.PDF);
                    doc.Close();
                }
            }
            catch (Exception)
            {

                throw;
            }
            return fdffn;
        }
    }
}
