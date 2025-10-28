using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Bold = DocumentFormat.OpenXml.Wordprocessing.Bold;
using BottomBorder = DocumentFormat.OpenXml.Wordprocessing.BottomBorder;
using Color = DocumentFormat.OpenXml.Wordprocessing.Color;
using Font = DocumentFormat.OpenXml.Wordprocessing.Font;
using Fonts = DocumentFormat.OpenXml.Wordprocessing.Fonts;
using FontSize = DocumentFormat.OpenXml.Wordprocessing.FontSize;
using InsideHorizontalBorder = DocumentFormat.OpenXml.Wordprocessing.InsideHorizontalBorder;
using InsideVerticalBorder = DocumentFormat.OpenXml.Wordprocessing.InsideVerticalBorder;
using Italic = DocumentFormat.OpenXml.Wordprocessing.Italic;
using LeftBorder = DocumentFormat.OpenXml.Wordprocessing.LeftBorder;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Path = DocumentFormat.OpenXml.Drawing.Path;
using RightBorder = DocumentFormat.OpenXml.Wordprocessing.RightBorder;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using TableCell = DocumentFormat.OpenXml.Wordprocessing.TableCell;
using TableCellBorders = DocumentFormat.OpenXml.Wordprocessing.TableCellBorders;
using TableCellProperties = DocumentFormat.OpenXml.Wordprocessing.TableCellProperties;
using TableProperties = DocumentFormat.OpenXml.Wordprocessing.TableProperties;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;
using TopBorder = DocumentFormat.OpenXml.Wordprocessing.TopBorder;

namespace FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.Controllers
{
        
        public static class OpenXmlWordHelpers
        {
            //  Pegar os campos campo das macros
            public static IEnumerable<FieldCode> GetMergeFields(this WordprocessingDocument doc, string mergeFieldName = null)
            {
                if (doc == null)
                    return null;

                List<FieldCode> mergeFields = doc.MainDocumentPart.RootElement.Descendants<FieldCode>().ToList();
                foreach (var header in doc.MainDocumentPart.HeaderParts)
                {
                    mergeFields.AddRange(header.RootElement.Descendants<FieldCode>());
                }

                foreach (var footer in doc.MainDocumentPart.FooterParts)
                {
                    mergeFields.AddRange(footer.RootElement.Descendants<FieldCode>());
                }

                if (!string.IsNullOrWhiteSpace(mergeFieldName) && mergeFields != null && mergeFields.Count() > 0)
                    return mergeFields.WhereNameIs(mergeFieldName);

                return mergeFields;
            }

            //  Pegar os campos campo das macros
            public static IEnumerable<SimpleField> GetSimpleFields(this WordprocessingDocument doc, string mergeFieldName = null)
            {
                if (doc == null)
                    return null;

                List<SimpleField> simpleFieldsF = doc.MainDocumentPart.RootElement.Descendants<SimpleField>().ToList();
                foreach (var header in doc.MainDocumentPart.HeaderParts)
                {
                    simpleFieldsF.AddRange(header.RootElement.Descendants<SimpleField>());
                }

                foreach (var footer in doc.MainDocumentPart.FooterParts)
                {
                    simpleFieldsF.AddRange(footer.RootElement.Descendants<SimpleField>());
                }

                if (!string.IsNullOrWhiteSpace(mergeFieldName) && simpleFieldsF != null && simpleFieldsF.Count() > 0)
                    return simpleFieldsF.WhereNameIsF(mergeFieldName);

                return simpleFieldsF;
            }

            /// Pega os campos das macros que estão em um determinado elemento do xml
            public static IEnumerable<FieldCode> GetMergeFields(this OpenXmlElement xmlElement, string mergeFieldName = null)
            {
                if (xmlElement == null)
                    return null;

                if (string.IsNullOrWhiteSpace(mergeFieldName))
                    return xmlElement
                        .Descendants<FieldCode>();

                return xmlElement
                    .Descendants<FieldCode>()
                    .Where(f => f.InnerText.StartsWith(GetMergeFieldStartString(mergeFieldName)));
            }

            public static string NameField(string mergeFieldName)
            {
                if (mergeFieldName.StartsWith(" MERGEFIELD"))
                {
                    Int32 endMerge = mergeFieldName.IndexOf("\\");
                    mergeFieldName = mergeFieldName.Substring(11, endMerge - 11);
                    mergeFieldName = mergeFieldName.Trim();
                }
                return mergeFieldName;
            }

            // pega uma macro com um determinado nome
            public static IEnumerable<FieldCode> WhereNameIs(this IEnumerable<FieldCode> mergeFields, string mergeFieldName)
            {
                if (mergeFields == null || mergeFields.Count() == 0)
                    return null;

                return mergeFields
                    .Where(f => f.InnerText.StartsWith(GetMergeFieldStartString(mergeFieldName)) &&
                            NameField(f.InnerText).Length == mergeFieldName.Length);
            }

            // pega um SimpleField com um determinado nome
            public static IEnumerable<SimpleField> WhereNameIsF(this IEnumerable<SimpleField> mergeFields, string mergeFieldName)
            {
                if (mergeFields == null || mergeFields.Count() == 0)
                    return null;

                return  mergeFields
                    .Where(f => f.Instruction.Value.StartsWith(GetMergeFieldStartString(mergeFieldName)) &&
                            NameField(f.Instruction.Value).Length == mergeFieldName.Length);
            }

            /// Pega o primeiro paragrafo que contido em um elemento do xml
            public static Paragraph GetParagraph(this OpenXmlElement xmlElement)
            {
                if (xmlElement == null)
                    return null;

                Paragraph paragraph = null;
                if (xmlElement is Paragraph)
                    paragraph = (Paragraph)xmlElement;
                else if (xmlElement.Parent is Paragraph)
                    paragraph = (Paragraph)xmlElement.Parent;
                else
                    paragraph = xmlElement.Ancestors<Paragraph>().FirstOrDefault();
                return paragraph;
            }

            /// Remove a macro(e seus componentes) e substitui pelo texto  this FieldCode field
            public static void ReplaceWithText(this OpenXmlElement field, string replacementText, bool setFonte = false)
            {
                if (field == null)
                    return;
                string fTexto = field.InnerText;
                if (fTexto.StartsWith(" MERGEFIELD"))
                    fTexto = '<' + NameField(fTexto) + '>';

                Run rFldCode = new Run();
                if (field as SimpleField == null)
                    rFldCode = field.Parent as Run;
                else 
                    rFldCode = field.GetFirstChild<Run>() as Run;

                Text t = rFldCode.GetFirstChild<Text>();
                while (t == null)
                {
                    rFldCode = rFldCode.NextSibling<Run>();
                    t = rFldCode.GetFirstChild<Text>();
                }

                Text t3 = t;
                Run rText = rFldCode;
                List<Run> runs = new List<Run>();
                //t.Text = rFldCode.InnerText;
                while (t.Text.Trim().Length != fTexto.Length)
                {
                    rText = rText.NextSibling<Run>();
                    if (rText != null)
                    {
                        t.Text = t.Text + rText.InnerText;
                        runs.Add(rText);
                    }
                    else break;
                }
                //Run rBegin = rFldCode.PreviousSibling<Run>();
                //Run rSep = rFldCode.NextSibling<Run>();
                //Run rText = rSep == null ? rFldCode : rSep.NextSibling<Run>();
                //Run rEnd = rText.NextSibling<Run>();
                //Run rEnd2 = rEnd == null ? rText : rEnd.NextSibling<Run>();

                if (setFonte)
                {
                    RunProperties pr = rFldCode.GetFirstChild<RunProperties>();//Era rText
                    if (pr != null) pr.Remove();
                    rFldCode.AppendChild(new RunProperties(//Era rText
                        new RunFonts() { Ascii = "Arial Narrow" },
                        new FontSize() { Val = "24" },
                        new Bold() { Val = OnOffValue.FromBoolean(true) }
                    ));
                }

                //Text t = rText.GetFirstChild<Text>();
                if (t3 != null)
                {
                    Text t2 = new Text();
                    //if (rSep != null)
                    //    t2 = rText.AppendChild(new Text(rEnd.InnerText != "" ? t.Text + rEnd.InnerText + rEnd2.InnerText : t.Text));
                    //else
                    //    t2 = rText.AppendChild(new Text(t.Text));
                    t2 = rFldCode.AppendChild(new Text(t.Text)); //Era rText
                    t3.Remove();
                    t2.Text = (replacementText != null) ? replacementText : string.Empty;
                }
                //if(rEnd != null && rEnd.InnerText != "")
                //    rEnd2.Remove();
                //rFldCode.Remove();
                //if (rBegin != null) rBegin.Remove();
                //if (rSep != null) rSep.Remove();
                //if (rEnd != null) rEnd.Remove();
                if (runs.Count > 0)
                    foreach (var runsD in runs)
                    {
                       runsD.Remove(); 
                    }
            }

            // Remove várias macros e substitui pelo texto
            public static void ReplaceWithText(this IEnumerable<FieldCode> fields, string replacementText, bool setfonte = false)
            {
                if (fields == null || fields.Count() == 0)
                    return;

                foreach (var field in fields)
                {
                    field.ReplaceWithText(replacementText, setfonte);
                }
            }

            // Remove várias macros e substitui pelo texto
            public static void ReplaceWithTextS(this IEnumerable<SimpleField> fields, string replacementText, bool setfonte = false)
            {
                if (fields == null || fields.Count() == 0)
                    return;

                foreach (var field in fields)
                {
                    field.ReplaceWithText(replacementText, setfonte);
                }
            }
            
            /// Removes uma lista de macros e substitui por uma lista de textos
            public static void ReplaceWithText(this IEnumerable<FieldCode> fields, IEnumerable<string> replacementTexts, bool removeExcess = false)
            {
                if (fields == null || fields.Count() == 0)
                    return;

                int replacementCount = replacementTexts.Count();
                int index = 0;
                foreach (var field in fields)
                {
                    if (index < replacementCount)
                        field.ReplaceWithText(replacementTexts.ElementAt(index));
                    else if (removeExcess)
                        field.GetParagraph().Remove();
                    else
                        field.ReplaceWithText(string.Empty);

                    index++;
                }
            }

            #region Private Methods
            // Tag da macro no template
            private static string GetMergeFieldStartString(string mergeFieldName)
            {
                return " MERGEFIELD  " + (!string.IsNullOrWhiteSpace(mergeFieldName) ? mergeFieldName : "<NoNameMergeField>");
            }

            //Cria uma celula na tabela
            public static TableCell CreateCell(string text)
            {
                return new TableCell(new Paragraph(new Run(new Text(text))));
            }

            public static GridColumn CreateHeaderCell()
            {
                return new GridColumn();
            }

            //configura as propriedades de uma uma celula para o docx
            public static TableCell ConfigureWidth(TableCell table, string width)
            {
                table.Append(new TableCellProperties(
                   new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = width }

                   //new TableCellTextStyle {Bold = BooleanStyleValues.On},
                   //new Fonts(new Font(new Bold(), new FontName() { Val = "Calibri" }))
                   // //new Bold { Val = OnOffValue.FromBoolean(true) }
                ));
                   /* 
                    new TableCellTextStyle {};
                    new Fonts
                    (
                        new Font(new Bold(), new FontName() {Val = "Calibri"})
                    );
                    new CellFormat() {FontId = 1, FillId = 0, BorderId = 0, ApplyFont = true};  // Index 1 - Bold.
                */
                 return table;
            }

            public static TableGrid CreateTableGrid(int cols)
            {
                List<GridColumn> columnsGrids = new List<GridColumn>();
                for (var i = 1; i <= cols; i++)
                {
                    columnsGrids.Add(CreateHeaderCell());
                }
                return new TableGrid(columnsGrids);
            }
            
            public static TableCell CreateCell1R(string text)
            {
                return new TableCell(new Paragraph(
                    new Run(
                        new RunProperties(new RunFonts() { Ascii = "Arial Narrow" }),
                        new RunProperties(new FontSize() { Val = "16" }),
                        new Text(text)
                    )
                    ));
            }
            
            public static TableCell CreateCell2R(string text, string text2)
            {
                return new TableCell(new Paragraph(
                    new Run(
                        new RunProperties(new RunFonts() { Ascii = "Arial Narrow" }),
                        new RunProperties(new FontSize() { Val = "16" }),
                        new Text(text)
                    ),
                    new Run(
                        new RunProperties(new RunFonts() { Ascii = "Arial Narrow" }),
                        new RunProperties(new FontSize() { Val = "24" }),
                        new RunProperties(new Bold()),
                        new Text(text2)
                    )
                    ));
            }

            public static TableCell CreateCell4R(string text, string text2, string text3, string text4)
            {
                return new TableCell(new Paragraph(
                    new Run(
                        new RunProperties(new RunFonts() { Ascii = "Arial Narrow" }),
                        new RunProperties(new FontSize() { Val = "16" }),
                        new Text(text)
                    ),
                    new Run(
                        new RunProperties(new RunFonts() { Ascii = "Arial Narrow" }),
                        new RunProperties(new FontSize() { Val = "24" }),
                        new RunProperties(new Bold()),
                        new Text(text2)
                    ),
                    new Run(
                        new RunProperties(new RunFonts() { Ascii = "Arial Narrow" }),
                        new RunProperties(new FontSize() { Val = "16" }),
                        new Text(text3)
                    ),
                    new Run(
                        new RunProperties(new RunFonts() { Ascii = "Arial Narrow" }),
                        new RunProperties(new FontSize() { Val = "24" }),
                        new RunProperties(new Bold()),
                        new Text(text4)
                    )
                    ));
            }

            public static TableCell ConfigureWidthPct(TableCell table, string width)
            {
                table.Append(new TableCellProperties(
                   new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = width }
                   ));
                return table;
            }

            public static TableCell ConfigureWidthMerge(TableCell table, string width, bool percent, bool mergeInicio)
            {
                table.Append(new TableCellProperties(
                   new TableCellWidth { Type = percent ? TableWidthUnitValues.Pct : TableWidthUnitValues.Dxa, Width = width },
                    new HorizontalMerge { Val = mergeInicio ? MergedCellValues.Restart : MergedCellValues.Continue }
                   ));
                return table;
            }

            public static TableCell ConfigureWidthSpan(TableCell table, string width, bool percent, int span)
            {
                table.Append(new TableCellProperties(
                   new TableCellWidth { Type = percent ? TableWidthUnitValues.Pct : TableWidthUnitValues.Dxa, Width = width },
                    new GridSpan { Val = span }
                   ));
                return table;
            }

            //configura borda na tabela para o docx
            public static TableCell ConfigureBorder(TableCell table)
            {
                table.Append(new TableCellProperties(
                    new TableCellBorders
                    {
                        BottomBorder = new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        TopBorder = new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        LeftBorder = new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        RightBorder = new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        InsideHorizontalBorder = new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                        InsideVerticalBorder = new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }

                    }
                    //new TableCellTextStyle {Bold = BooleanStyleValues.On},
                    //new Fonts(new Font(new Bold(), new FontName() { Val = "Calibri" }))
                    // //new Bold { Val = OnOffValue.FromBoolean(true) }

                ));


                /* 
                 new TableCellTextStyle {};
                 new Fonts
                 (
                     new Font(new Bold(), new FontName() {Val = "Calibri"})
                 );
                 new CellFormat() {FontId = 1, FillId = 0, BorderId = 0, ApplyFont = true};  // Index 1 - Bold.
                    
             */

                return table;
            }

            public static void SetTableStyle(Table table)
            {
                TableProperties properties = new TableProperties();

                //table borders
                TableBorders borders = new TableBorders();

                borders.TopBorder = new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Dotted), Size = 5 };
                borders.BottomBorder = new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Dotted), Size = 5 };
                borders.LeftBorder = new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Dotted), Size = 5 };
                borders.RightBorder = new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Dotted), Size = 5 };
                borders.InsideHorizontalBorder = new InsideHorizontalBorder() { Val = BorderValues.Dotted, Size = 5 };
                borders.InsideVerticalBorder = new InsideVerticalBorder() { Val = BorderValues.Dotted, Size = 5 };

                properties.Append(borders);

                //set the table width to page width
                TableWidth tableWidth = new TableWidth() { Width = "100%", Type = TableWidthUnitValues.Pct };
                properties.Append(tableWidth);

                //add properties to table
                table.Append(properties);
            }





            //Outra forma de substituir as macros (muito geral, não funcionou muito bem)

            public static char[] splitChar = new char[] { ' ' };
            public static void Replace(WordprocessingDocument document, Dictionary<string, string> replaceOperations)
            {
                foreach (var field in document.MainDocumentPart.Document.Body.Descendants<SimpleField>())
                {
                    string[] instruction = field.Instruction.Value.Split(splitChar, StringSplitOptions.RemoveEmptyEntries);

                    if (instruction[0].ToLower().Equals("mergefield"))
                    {
                        string fieldname = instruction[1];

                        foreach (var fieldtext in field.Descendants<Text>())
                        {
                            string value = replaceOperations.ContainsKey(fieldname) ? replaceOperations[fieldname] : null;

                            if (value != null)
                                fieldtext.Text = value;

                            break;
                        }
                    }
                }


            }

            //Outra forma de substituir as macros (muito geral, não funcionou muito bem)
            public static void FindField(WordprocessingDocument document, Dictionary<string, string> replaceOperations)
            {
                foreach (var field in document.MainDocumentPart.Document.Body.Descendants<SimpleField>())
                {
                    string[] instruction = field.Instruction.Value.Split(splitChar, StringSplitOptions.RemoveEmptyEntries);

                    if (instruction[0].ToLower().Equals("mergefield"))
                    {
                        string fieldname = instruction[1];

                        foreach (var fieldtext in field.Descendants<Text>())
                        {
                            
                            string value = replaceOperations.ContainsKey(fieldname) ? replaceOperations[fieldname] : null;

                            if (value != null)
                                fieldtext.Text = value;

                            break;
                        }
                    }
                }


            }

            //teste para converter para pdf usando power shell(precisa ter o word instalado)
            public static void ToPdfFolder()
            {
                var ps1File = @"E:\Projetos\SGF-trunk-sm\FundacaoFisk.SGF.Web\Content\Uploads\TempContratos\Convert-Documents.ps1";

                var startInfo = new ProcessStartInfo()
                {
                    FileName = "powershell.exe",
                    
                    Arguments = "-NoProfile -ExecutionPolicy unrestricted -file \"&'"+ps1File+"'\"",
                    UseShellExecute = false
                };
              Process process =  Process.Start(startInfo);
              process.WaitForExit();
            
              // Check for failed exit code.
              if (process.ExitCode != 0) {
                  throw new Exception(process.ExitCode.ToString());
              }
            }
            #endregion Private Methods


        }


    
}
