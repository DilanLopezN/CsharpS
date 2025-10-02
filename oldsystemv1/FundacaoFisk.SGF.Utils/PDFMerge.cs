using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Utils
{
    public class PDFMerge
    {
        /// <summary>
        /// Lista de arquivos a serem concatenados
        /// </summary>
        private List<string> pdfList;

        /// <summary>
        /// Objeto que representa um documento (pdf) do iTextSharp
        /// </summary>
        private Document document;

        /// <summary>
        /// Objeto responsável por salvar o pdf em disco.
        /// </summary>
        private PdfWriter writer;

        /// <summary>
        /// Construtor
        /// </summary>
        public PDFMerge()
        {
            pdfList = new List<string>();
        }

        public PDFMerge(List<string> diretorios)
        {
            pdfList = diretorios;
        }

        /// <summary>
        /// Adiciona o arquivo que será concatenado ao PDF final.
        /// </summary>
        /// Caminho para o arquivo PDF
        public void Add(string filePath)
        {
            pdfList.Add(filePath);
        }

        /// <summary>
        /// Adiciona uma lista de arquivos pdf para serem concatenados.
        /// </summary>
        /// Lista contendo o caminho para os arquivos
        public void AddList(List<string> files)
        {
            pdfList.AddRange(files);
        }

        /// <summary>
        /// Concatena os arquivos de entrada, gerando um novo arquivo PDF.
        /// </summary>
        public void Save(string pathToDestFile)
        {
            PdfReader reader = null;
            PdfContentByte cb = null;
            int index = 0;
            try
            {
                // Percorre a lista de arquivos a serem concatenados.
                foreach (string file in pdfList)
                {
                   // FileStream st = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    //byte[] buffer = new byte[st.Length];
                    //st.Close();
                    if (reader != null)
                    {
                        reader.Close();
                    }
                    reader = new PdfReader(new RandomAccessFileOrArray(file), null);
                    //reader = new PdfReader(buffer);
                    // Cria o PdfReader para ler o arquivo
                    //reader = new PdfReader(pdfList[index]);
                    // Obtém o número de páginas deste pdf
                    int numPages = reader.NumberOfPages;

                    if (index == 0)
                    {
                        // Cria o objeto do novo documento
                        document = new Document(reader.GetPageSizeWithRotation(1));
                        // Cria um writer para gravar o novo arquivo
                       // writer = PdfWriter.GetInstance(document, ms);
                        
                        writer = PdfWriter.GetInstance(document, new FileStream(pathToDestFile, FileMode.Create));
                        // Abre o documento
                        document.Open();
                        cb = writer.DirectContent;
                    }

                    // Adiciona cada página do pdf origem ao pdf destino.
                    int i = 0;
                    while (i < numPages)
                    {
                        i++;
                        document.SetPageSize(reader.GetPageSizeWithRotation(i));
                        document.NewPage();
                        PdfImportedPage page = writer.GetImportedPage(reader, i);
                        int rotation = reader.GetPageRotation(i);
                        if (rotation == 90 || rotation == 270)
                            cb.AddTemplate(page, 0, -1f, 1f, 0, 0, reader.GetPageSizeWithRotation(i).Height);
                        else
                            cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                    }
                          
                    index++;
                }
                //document.Dispose();
            }
            catch (Exception ex)
            {
                // Tratar a exceção de acordo com a necessidade de cada projeto.
                // Não vamos realizar o tratamento da exceção de forma mais elaborada 
                // por motivos didáticos.
                throw ex;
            }
            finally
            {
                if (document != null)
                {
                    document.Dispose();
                    document.Close();
                }
                if (reader != null)
                {
                    reader.Dispose();
                    reader.Close();
                }
            }
        }

        public static void CombineMultiplePDFs(List<string> fileNames, string outFile)
        {
            // step 1: creation of a document-object
            Document document = new Document();

            // step 2: we create a writer that listens to the document
            PdfCopy writer = new PdfCopy(document, new FileStream(outFile, FileMode.Create));
            if (writer == null)
            {
                return;
            }

            // step 3: we open the document
            document.Open();

            foreach (string fileName in fileNames)
            {
                // we create a reader for a certain document
                PdfReader reader = new PdfReader(fileName);
                reader.ConsolidateNamedDestinations();

                // step 4: we add content
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    PdfImportedPage page = writer.GetImportedPage(reader, i);
                    writer.AddPage(page);
                }

                //PRAcroForm form = reader.AcroForm;
                //if (form != null)
                //{
                //    writer.CopyAcroForm(reader);
                //}

                reader.Close();
            }

            // step 5: we close the document and writer
            writer.Close();
            document.Close();
            document.Dispose();
        }

    }
}
