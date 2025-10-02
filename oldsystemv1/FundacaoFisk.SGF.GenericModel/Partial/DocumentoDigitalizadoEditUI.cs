using System;
using Componentes.GenericModel;
using System.Collections.Generic;

namespace FundacaoFisk.SGF.GenericModel
{
    public class DocumentoDigitalizadoEditUI : TO
    {
        public int cd_contrato { get; set; }
        public Nullable<int> nm_contrato { get; set; }
        public string nm_arquivo_digitalizado { get; set; }
        public string nm_arquivo_digitalizado_temporario { get; set; }
        public int cd_pessoa_escola { get; set; }
    }

    public class PacoteCertificadoUI : TO
    {
        public int cd_contrato { get; set; }
        public Nullable<int> nm_contrato { get; set; }
        public bool id_venda_pacote { get; set; }
        public bool id_liberar_certificado { get; set; }
        public List<CursosContrato> CursoContrato { get; set; }
    }

    public class CursosContrato
    {
        public int cd_curso_contrato { get; set; }
        public int cd_contrato { get; set; }
        public int cd_curso { get; set; }
        public bool id_liberar_certificado { get; set; }
    }
}