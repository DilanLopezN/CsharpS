//#region Métodos auxiliares do formulario - carregarMascara() 

//Declarando as variável para retorno da FK de aluno.
//Diario Aula
var PESQUISADIARIOAULA = 1, CADDIARIOAULA = 2;
//Avaliação
var PESQUISAAVALIACAO = 3, CADAVALIACAO = 4;
//Controle de Falta
var CONTROLEFALTA = 25;
//Controle de Falta Incluir
var CONTROLEFALTAGRIDINCLUIR = 26;
//Desistencia
var PESQUISADESISTENCIA = 5, CADDESISTENCIA = 6;
//Turma
var CADASTROTURMA = 8;
//Mudanças Internas
var MUDANCAINTERNA = 9;
//matrícula
var CADMATRICULA = 10;
//Turma FK
var TURMAFK = 11;
//Atividade Extra
var ATIVIDADEEXTRA = 12;
//Historico Aluno
var HISTORICOALUNO = 13;
// Retorno CNAB
var RETORNOCNAB = 14;
//CNAB Boleto
var CADCNABBOLETO = 15;
//Relatório Matrícula
var RELMATRICULA = 16;
//Relatório de Cheques
var RELCHEQUES = 17;
//Politica de Desconto
var POLITICADESCONTOALUNO = 18;
//Relatório de Aula Personalizada
var PESQUISAAULAPERSONALIZADA = 19, CADAULAPERSONALIZADA = 20;
//Movimento
var CADMOVIMENTO = 21;
//Movimento
var REAJUSTE_ANUAL_ALUNO = 22;
//Relatorio Turma x Matrícula x Material
var RELTURMAMATRICULAMATERIAL = 23;

var AULAREPOSICAO = 28, AULAREPOSICAOCADASTRO = 29;
var ENVIARTRANSFERENCIA = 30, ENVIARTRANSFERENCIACAD = 31;

var ALUNOSEMAULA = 33

var DESISTENCIA_CARGA = 50;
var CARGA_HORARIA = 42; CARGA_HORARIA_COM_ESCOLA = 43;

var ORIGEM_ALUNO_SEARCH_PERDA_MATERIAL = 44, ORIGEM_ALUNO_FK_MOVIMENTO = 45;

function carregarMascara() {
    try {
        if (hasValue(dojo.byId("cpf_fk")))
            $("#cpf_fk").mask("999.999.999-99");
    }
    catch (e) {
        postGerarLog(e);
    }

}

function limparPesquisaAlunoFK() {
    try {
        dojo.byId("nomeAlunoFk").value = "";
        dojo.byId("emailPesqFK").value = "";
        dijit.byId("inicioAlunoFK").reset();
        dijit.byId("statusAlunoFK").set("value", 0);
        dijit.byId("nm_sexo_fk").set("value", 0);
        dojo.byId('cpf_fk').value = "";
        if (hasValue(dijit.byId("gridPesquisaAluno")) && hasValue(dijit.byId("gridPesquisaAluno").itensSelecionados))
            dijit.byId("gridPesquisaAluno").itensSelecionados = [];
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparPesquisaCursoFK() {
    try {
        dojo.byId("nomeAlunoFk").value = "";
        dojo.byId("emailPesqFK").value = "";
        dijit.byId("inicioAlunoFK").reset();
        dijit.byId("statusAlunoFK").set("value", 0);
        dijit.byId("nm_sexo_fk").set("value", 0);
        dojo.byId('cpf_fk').value = "";
        if (hasValue(dijit.byId("gridPesquisaAluno")) && hasValue(dijit.byId("gridPesquisaAluno").itensSelecionados))
            dijit.byId("gridPesquisaAluno").itensSelecionados = [];
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region criando grade da fk - montarGridPesquisaAluno() - formatCheckBoxAlunoFK

function formatCheckBoxAlunoFK(value, rowIndex, obj) {
    try {
        var gridName = 'gridPesquisaAluno';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosAlunoFK');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_aluno", grid._by_idx[rowIndex].item.cd_aluno);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_aluno', 'selecionadoAlunoFK', -1, 'selecionaTodosAlunoFK', 'selecionaTodosAlunoFK', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_aluno', 'selecionadoAlunoFK', " + rowIndex + ", '" + id + "', 'selecionaTodosAlunoFK', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarStatusAlunoFK(nomElement, simples) {
    require(["dojo/store/Memory", "dijit/form/FilteringSelect"],
     function (Memory, filteringSelect) {
         try {
             var statusStore = null;

             if (simples)
                 statusStore = new Memory({
                     data: [
                         { name: "Ativos", id: "1" },
                         { name: "Inativos", id: "2" }
                     ]
                 });
             else
                 statusStore = new Memory({
                     data: [
                         { name: "Todos", id: "0" },
                         { name: "Ativos", id: "1" },
                         { name: "Inativos", id: "2" }
                     ]
                 });
             var status = new filteringSelect({
                 id: nomElement,
                 name: "status",
                 value: "1",
                 store: statusStore,
                 searchAttr: "name",
                 style: "width: 75px;"
             }, nomElement);
         }
         catch (e) {
             postGerarLog(' URL: = ' + window.location.href + ' ' + e);
         }
     });
};

function montarGridPesquisaAluno(paginada, funcao) {
    require([
       "dojox/grid/EnhancedGrid",
       "dojox/grid/enhanced/plugins/Pagination",
       "dojo/data/ObjectStore",
       "dojo/store/Memory",
       "dijit/form/Button",
       "dojo/ready",
       "dojo/on"
    ], function (EnhancedGrid, Pagination, ObjectStore, Memory, Button, ready, on) {
        ready(function () {
            try {
                /*findAllEmpresasUsuarioComboFK();

                dojo.byId("lbEscolaAlunoFk").style.display = "none";
                require(['dojo/dom-style', 'dijit/registry'],
                    function (domStyle, registry) {

                        domStyle.set(registry.byId("escolaAlunoFK").domNode, 'display', 'none');
                    });*/

                loadPesqSexo(Memory, dijit.byId("nm_sexo_fk"));
                montarStatusAlunoFK("statusAlunoFK");
                var myStore = null;
                var store = null
                store = new ObjectStore({ objectStore: new Memory({ data: null }) });

                //*** Cria a grade de Cursos **\\
                var gridAlunoFK = new EnhancedGrid({
                    store: store,
                    structure:
                            [
                                { name: "<input id='selecionaTodosAlunoFK' style='display:none'/>", field: "selecionadoAlunoFK", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxAlunoFK },
                                { name: "Nome", field: "no_pessoa", width: "25%", styles: "min-width:80px;" },
                                { name: "CPF\\CNPJ", field: "nm_cpf_dependente", width: "25%" },
                                { name: "E-mail", field: "email", width: "80px" },
                                { name: "Data Cadastro", field: "dta_cadastro", width: "80px", styles: "min-width:80px;" },
                                { name: "Ativo", field: "id_ativo", width: "50px", styles: "text-align: center; min-width:50px; max-width: 50px;" }
                            ],
                    noDataMessage: msgNotRegEnc,
                    plugins: {
                        pagination: {
                            pageSizes: ["15", "30", "45", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "15",
                            gotoButton: true,
                            maxPageStep: 5,
                            position: "button",
                            plugins: { nestedSorting: true }
                        }
                    }
                }, "gridPesquisaAluno");
                gridAlunoFK.on("RowDblClick", function () {}, true);
                gridAlunoFK.canSort = function (col) { return Math.abs(col) == 2 };
                gridAlunoFK.pagination.plugin._paginator.plugin.connect(gridAlunoFK.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    try {
                        verificaMostrarTodos(evt, gridAlunoFK, 'cd_aluno', 'selecionaTodosAlunoFK');
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridAlunoFK, "_onFetchComplete", function () {
                        try {
                            // Configura o check de todos:
                            if (hasValue(dojo.byId('selecionaTodosAlunoFK'), true) && dojo.byId('selecionaTodosAlunoFK').type == 'text')
                                setTimeout("configuraCheckBox(false, 'cd_aluno', 'selecionadoAlunoFK', -1, 'selecionaTodosAlunoFK', 'selecionaTodosAlunoFK', 'gridPesquisaAluno')", gridAlunoFK.rowsPerPage * 3);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    });
                });



                gridAlunoFK.startup();

                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () { dijit.byId("proAluno").hide(); }
                }, "fecharAlunoFK");
                decreaseBtn(document.getElementById("pesquisarAlunoFK"), '32px');
                adicionarAtalhoPesquisa(['nomeAlunoFk', 'emailPesqFK', 'statusAlunoFK', 'cpf_fk', 'nm_sexo_fk'], 'pesquisarAlunoFK', ready);
                dijit.byId("pesquisarAlunoFK").on("click", function () {
                    try {
                        var tipo = parseInt(dojo.byId('tipoRetornoAlunoFK').value);
                        switch (tipo) {
                            case PESQUISADIARIOAULA:
                            case CADDIARIOAULA:
                            case PESQUISAAVALIACAO:
                            case CADAVALIACAO: {
                                pesquisarAlunoFK(true);
                                break;
                            }
                            case ENVIARTRANSFERENCIA:
                            {
                                pesquisarAlunoFK(true, ENVIARTRANSFERENCIA);
                                break;
                            }
                            case ORIGEM_ALUNO_FK_MOVIMENTO:
                            {
                                pesquisarAlunoFK(true, ORIGEM_ALUNO_FK_MOVIMENTO);
                                break;
                            }
                            case ORIGEM_ALUNO_SEARCH_PERDA_MATERIAL:
                            {
                                pesquisarAlunoFK(true, ORIGEM_ALUNO_SEARCH_PERDA_MATERIAL);
                                break;
                            }

                            case ENVIARTRANSFERENCIACAD: {
                                pesquisarAlunoFK(true, ENVIARTRANSFERENCIACAD);
                                break;
                            }
                            case CADASTROTURMA: {
                                chamarPesquisaTurmaFKTurma();
                                break;
                            }
                            case TURMAFK: {
                                pesquisaAlunoTurmaFKTurmaFK();
                                break;
                            }
                            case CADMATRICULA:{
                                pesquisarAlunoFKMatricula(true);
                                break;
                            }
                            case ATIVIDADEEXTRA: {
                                if (hasValue(pesquisarAlunoFK)) {
                                    apresentaMensagem('apresentadorMensagem', null);
                                    if (hasValue(dijit.byId("cbCursos").value)) {
                                        var cursos = dijit.byId("cbCursos").value;
                                    } 
                                    
                                    pesquisarAlunoFKAtividadeExtra(true, cursos);
                                }
                                break;
                            }
                            case AULAREPOSICAO: {
                                if (hasValue(pesquisarAlunoFK)) {
                                    apresentaMensagem('apresentadorMensagem', null);

                                    pesquisarAlunoFK(true, AULAREPOSICAO);
                                }
                                break;
                            }
                            case ALUNOSEMAULA: {
                                if (hasValue(pesquisarAlunoFK)) {
                                    apresentaMensagem('apresentadorMensagem', null);

                                    pesquisarAlunoFK(true, ALUNOSEMAULA);
                                }
                                break;
                            }
                            case DESISTENCIA_CARGA: {
                                if (hasValue(pesquisarAlunoFK)) {
                                    apresentaMensagem('apresentadorMensagem', null);

                                    pesquisarAlunoFK(true, DESISTENCIA_CARGA);
                                }
                                break;
                            }
                            case CARGA_HORARIA: {
                                if (hasValue(pesquisarAlunoFK)) {
                                    apresentaMensagem('apresentadorMensagem', null);

                                    pesquisarAlunoFK(true, CARGA_HORARIA);
                                }
                                break;
                            }
                            case CARGA_HORARIA_COM_ESCOLA: {
                                if (hasValue(pesquisarAlunoFK)) {
                                    apresentaMensagem('apresentadorMensagem', null);

                                    pesquisarAlunoFK(true, CARGA_HORARIA_COM_ESCOLA);
                                }
                                break;
                            }
                            case AULAREPOSICAOCADASTRO: {
                                if (hasValue(pesquisarAlunoFKAulaRep)) {
                                    apresentaMensagem('apresentadorMensagem', null);

                                    pesquisarAlunoFKAulaRep(true, AULAREPOSICAOCADASTRO,0);
                                }
                                break;
                            }

                            case RELCHEQUES:
                            case RETORNOCNAB:
                            case HISTORICOALUNO:
                            case CADCNABBOLETO:
                            case POLITICADESCONTOALUNO: {
                                if (hasValue(pesquisarAlunoFK)) {
                                    apresentaMensagem('apresentadorMensagem', null);
                                    pesquisarAlunoFK(true);
                                }
                                break;
                            }
                            case CADMOVIMENTO:{
                                pesquisarAlunoFKMovimento(true);
                                break;
                            }
                            case RELTURMAMATRICULAMATERIAL: {
                                if (hasValue(pesquisarAlunoFK)) {
                                    apresentaMensagem('apresentadorMensagem', null);
                                    getAlunoOrigemMaterial(true);
                                }
                                break;
                            }
                            case REAJUSTE_ANUAL_ALUNO: {
                                if (hasValue(pesquisaAlunoReajusteAnual)) {
                                    apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
                                    pesquisaAlunoReajusteAnual(true);
                                }
                                break;
                            }
                            case PESQUISADESISTENCIA: {
                                if (hasValue(pesquisaAlunoDesistente)) {
                                    apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
                                    pesquisaAlunoDesistente();
                                }
                                break;
                            }
                            case CADDESISTENCIA: {
                                if (hasValue(pesquisaAlunoDesistFK)) {
                                    apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
                                    pesquisaAlunoDesistFK();
                                }
                                break;
                            }
                            case RELMATRICULA: {
                                if (hasValue(pesquisaAlunoFKRel)) {
                                    apresentaMensagem('apresentadorMensagem', null);
                                    pesquisaAlunoFKRel();
                                }
                                break;
                            }
                            case PESQUISAAULAPERSONALIZADA: {
                                if (hasValue(pesquisarAlunoFK)) {
                                    apresentaMensagem('apresentadorMensagem', null);
                                    pesquisarAlunoFK(true);
                                }
                                break;
                            }
                            case CADAULAPERSONALIZADA: {
                                if (hasValue(pesquisarAlunoFK)) {
                                    apresentaMensagem('apresentadorMensagem', null);
                                    pesquisarAlunoFKAulaPersonalizada(true);
                                }
                                break;
                            }
                            default: caixaDialogo(DIALOGO_INFORMACAO, "Nenhum tipo de retorno foi selecionado/encontrado.");
                                return false;
                                break;
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("selecionaAlunoFK").on("click", function () {
                    try {
                        var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");
                        var tipo = parseInt(dojo.byId('tipoRetornoAlunoFK').value);

                        if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0) {
                            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                            return false;
                        }
                        else if (!(tipo == REAJUSTE_ANUAL_ALUNO) && gridPesquisaAluno.itensSelecionados.length > 1 &&
                            parseInt(dojo.byId('tipoRetornoAlunoFK').value) != POLITICADESCONTOALUNO &&
                            tipo !== ATIVIDADEEXTRA && 
                            tipo !== AULAREPOSICAOCADASTRO &&
                            tipo !== AULAREPOSICAO && 
                            tipo !== CADASTROTURMA &&
                            tipo !== PESQUISAAULAPERSONALIZADA &&
                            tipo !== CADAULAPERSONALIZADA
                                ) {
                            caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
                            return false;
                        }
                        else {
                            switch (tipo) {
                                case PESQUISADIARIOAULA: {
                                    if (hasValue(retornoPadraoAlunoFKParaTurmaFK))
                                        retornoPadraoAlunoFKParaTurmaFK(gridPesquisaAluno);
                                    break;
                                }
                                case ENVIARTRANSFERENCIA: {
                                    if (hasValue(retornoPadraoAlunoFKParaTurmaFK))
                                        retornarAlunoEnviarTranferencia(gridPesquisaAluno);
                                    break;
                                }
                                case ENVIARTRANSFERENCIACAD: {
                                    if (hasValue(retornoPadraoAlunoFKParaTurmaFK))
                                        retornarAlunoEnviarTranferenciaCad(gridPesquisaAluno);
                                    break;
                                }
                                case CADDIARIOAULA: {
                                    if (hasValue(retornoPadraoAlunoFKParaTurmaFK))
                                        retornoPadraoAlunoFKParaTurmaFK(gridPesquisaAluno);
                                    break;
                                }
                                case PESQUISAAVALIACAO: {
                                    if (hasValue(retornoPadraoAlunoFKParaTurmaFK))
                                        retornoPadraoAlunoFKParaTurmaFK(gridPesquisaAluno);
                                    break;
                                }
                                case CADAVALIACAO: {
                                    if (hasValue(retornoPadraoAlunoFKParaTurmaFK))
                                        retornoPadraoAlunoFKParaTurmaFK(gridPesquisaAluno);
                                    break;
                                }
                                case CADDESISTENCIA: {
                                    retornarAlunoCadFK();
                                    break;
                                }
                                case DESISTENCIA_CARGA:
                                case ALUNOSEMAULA: {
                                    retornarAlunoFK();
                                    break;
                                }
                                case CARGA_HORARIA:
                                case CARGA_HORARIA_COM_ESCOLA: {
                                    retornarAlunoFK();
                                    break;
                                }
                                case ORIGEM_ALUNO_SEARCH_PERDA_MATERIAL: {
                                    retornarAlunoFK(ORIGEM_ALUNO_SEARCH_PERDA_MATERIAL);
                                    break;
                                }
                                case ORIGEM_ALUNO_FK_MOVIMENTO: {
                                    retornarAlunoFK(ORIGEM_ALUNO_FK_MOVIMENTO);
                                    break;
                                }
                                case MUDANCAINTERNA: {
                                    if (hasValue(retornoPadraoAlunoFKParaTurmaFK))
                                        retornoPadraoAlunoFKParaTurmaFK(gridPesquisaAluno);
                                    break;
                                }
                                case CADASTROTURMA: {
                                    if (hasValue(retornarAlunoFKTurma))
                                        retornarAlunoFKTurma();
                                    break;
                                }
                                case AULAREPOSICAOCADASTRO:
                                case AULAREPOSICAO: {
                                    if (hasValue(retornarAlunoFKAtivdadeExtra))
                                        retornarAlunoFKAtivdadeExtra();
                                    break;
                                }
                                case ATIVIDADEEXTRA: {
                                    if (hasValue(retornarAlunoFKAtivdadeExtra))
                                        retornarAlunoFKAtivdadeExtra();
                                    break;
                                }
                                case TURMAFK: {
                                    if (hasValue(retornoPadraoAlunoFKParaTurmaFK))
                                        retornoPadraoAlunoFKParaTurmaFK(gridPesquisaAluno);
                                    break;
                                }
                                case HISTORICOALUNO: {
                                    if (hasValue(retornarAlunoHistoricoFK))
                                        retornarAlunoHistoricoFK();
                                    break;
                                }
                                case RELCHEQUES:
                                case PESQUISADESISTENCIA:
                                case CADMATRICULA:
                                case RELTURMAMATRICULAMATERIAL:
                                case RETORNOCNAB:
                                case CADCNABBOLETO:
                                case CADMOVIMENTO:
                                case REAJUSTE_ANUAL_ALUNO:
                                case POLITICADESCONTOALUNO: {
                                    if (hasValue(retornarAlunoFK)) {
                                        var selecionaVariosRegistros = tipo == REAJUSTE_ANUAL_ALUNO ? true : false;
                                        retornarAlunoFK(selecionaVariosRegistros);
                                    }
                                    break;
                                }
                                //case REAJUSTE_ANUAL_ALUNO: {
                                //    if (hasValue(retornarAlunoFK))
                                //        retornarAlunoFK(false);
                                //    break;
                                //}
                                case RELMATRICULA: {
                                    if (hasValue(retornarAlunoRelFK))
                                        retornarAlunoRelFK();
                                    break;
                                }
                                case CONTROLEFALTA: {
                                        if (hasValue(retornarAlunoControleFaltaFK))
                                            retornarAlunoControleFaltaFK();
                                        break;
                                }
                                case CONTROLEFALTAGRIDINCLUIR: {
                                    if (hasValue(retornarAlunoFKGridIncluirControleFalta))
                                        retornarAlunoFKGridIncluirControleFalta();
                                    break;
                                }
                                case PESQUISAAULAPERSONALIZADA: {
                                    if (hasValue(retornarAlunoFKPesqAulaPersonalizada))
                                        retornarAlunoFKPesqAulaPersonalizada();
                                    break;
                                }
                                case CADAULAPERSONALIZADA: {
                                    if (hasValue(retornarAlunoFKAulaPer))
                                        retornarAlunoFKAulaPer();
                                    break;
                                }
                                default: caixaDialogo(DIALOGO_INFORMACAO, "Nenhum tipo de retorno foi selecionado/encontrado.");
                                    return false;
                                    break;
                            }
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("nm_sexo_fk").on("change", function (event) {
                    try {
                        var TODOS = 0;
                        if (!hasValue(event) || event < TODOS)
                            dijit.byId("nm_sexo_fk").set("value", TODOS);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("statusAlunoFK").on("change", function (event) {
                    try {
                        var TODOS = 0;
                        if (!hasValue(event) || event < TODOS)
                            dijit.byId("statusAlunoFK").set("value", TODOS);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                if (hasValue(funcao))
                    funcao.call();
                carregarMascara();
            }
            catch (e) {
                postGerarLog(e);
            }
        });

    });
}
//#endregion

//#region métodos para  o aluno - pesquisarAlunoFK
function pesquisarAlunoFK(pesquisaHabilitada,origem) {
    try {
        var sexo = hasValue(dijit.byId("nm_sexo_fk").value) ? dijit.byId("nm_sexo_fk").value : 0;
        var origemFK = hasValue(origem) ? origem : 0;
        if (pesquisaHabilitada)
            require([
                    "dojo/store/JsonRest",
                    "dojo/data/ObjectStore",
                    "dojo/store/Cache",
                    "dojo/store/Memory"
            ], function (JsonRest, ObjectStore, Cache, Memory) {
                try {
                    myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/aluno/getAlunoSearchFKPesquisas?nome=" + dojo.byId("nomeAlunoFk").value + "&email=" + dojo.byId("emailPesqFK").value + "&status=" + dijit.byId("statusAlunoFK").value + "&cnpjCpf=" + dojo.byId("cpf_fk").value + "&inicio=" +
                            document.getElementById("inicioAlunoFK").checked + "&origemFK=" + origemFK + "&cdSituacoes=100&sexo=" + sexo + "&semTurma=false&movido=false&tipoAluno=0&cd_pessoa_responsavel=0",
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        idProperty: "cd_aluno"
                    }), Memory({ idProperty: "cd_aluno" }));
                    var dataStore = new ObjectStore({ objectStore: myStore });
                    var gridAluno = dijit.byId("gridPesquisaAluno");
                    gridAluno.itensSelecionados = [];
                    gridAluno.setStore(dataStore);
                }
                catch (e) {
                    postGerarLog(e);
                }
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarAlunoAtividadeFK(pesquisaHabilitada, origem) {
    try {

        var sexo = hasValue(dijit.byId("nm_sexo_fk").value) ? dijit.byId("nm_sexo_fk").value : 0;
        var origemFK = hasValue(origem) ? origem : 0;
        if (pesquisaHabilitada)
            require([
                "dojo/store/JsonRest",
                "dojo/data/ObjectStore",
                "dojo/store/Cache",
                "dojo/store/Memory"
            ], function (JsonRest, ObjectStore, Cache, Memory) {
                try {
                    var cdsEscolas = (dojo.byId("tagEscola").style.display === "none") ? "" : montarStringCdsEscolas();
                    /*
                    if (!hasValue(cdsEscolas)) {
                        dijit.byId("escolaAlunoFK").set("value", dojo.byId("_ES0").value);
                        dojo.byId("lbEscolaAlunoFk").style.display = "none";
                        require(['dojo/dom-style', 'dijit/registry'],
                            function (domStyle, registry) {
                                domStyle.set(registry.byId("escolaAlunoFK").domNode, 'display', 'none');
                            });
                    } else {

                        dojo.byId("lbEscolaAlunoFk").style.display = "";
                        require(['dojo/dom-style', 'dijit/registry'],
                            function (domStyle, registry) {
                                domStyle.set(registry.byId("escolaAlunoFK").domNode, 'display', '');
                            });
                    }
                    if (dijit.byId("escolaAlunoFK").value == "")
                        dijit.byId("escolaAlunoFK").set("value", dojo.byId("_ES0").value);*/
                    myStore = Cache(
                        JsonRest({
                            target: Endereco() + "/api/aluno/getAlunoSearchFKPesquisasAtividade?nome=" + dojo.byId("nomeAlunoFk").value + "&email=" + dojo.byId("emailPesqFK").value + "&status=" + dijit.byId("statusAlunoFK").value + "&cnpjCpf=" + dojo.byId("cpf_fk").value + "&inicio=" +
                                document.getElementById("inicioAlunoFK").checked + "&origemFK=" + origemFK + "&cdSituacoes=100&sexo=" + sexo + "&semTurma=false&movido=false&tipoAluno=0&cd_escola_combo_fk=0" + "&cd_pessoa_responsavel=0&cdEscolas=" + cdsEscolas,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                            idProperty: "cd_aluno"
                        }), Memory({ idProperty: "cd_aluno" }));
                    var dataStore = new ObjectStore({ objectStore: myStore });
                    var gridAluno = dijit.byId("gridPesquisaAluno");
                    gridAluno.itensSelecionados = [];
                    gridAluno.setStore(dataStore);
                }
                catch (e) {
                    postGerarLog(e);
                }
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarAlunoFKAulaRep(pesquisaHabilitada,origem,turma) {
    try {
        var sexo = hasValue(dijit.byId("nm_sexo_fk").value) ? dijit.byId("nm_sexo_fk").value : 0;
        var origemFK = hasValue(origem) ? origem : 0;
        var turmaFK = hasValue(turma) ? turma : 0;
        if (pesquisaHabilitada)
            require([
                    "dojo/store/JsonRest",
                    "dojo/data/ObjectStore",
                    "dojo/store/Cache",
                    "dojo/store/Memory"
            ], function (JsonRest, ObjectStore, Cache, Memory) {
                try {
                    myStore = Cache(
                    JsonRest({ 
                        target: Endereco() + "/api/aluno/getAlunoPorTurmaSearchAulaReposicao?nome=" + dojo.byId("nomeAlunoFk").value + "&email=" + dojo.byId("emailPesqFK").value + "&status=" + dijit.byId("statusAlunoFK").value + "&Cpf=" + dojo.byId("cpf_fk").value + "&inicio=" +
                            document.getElementById("inicioAlunoFK").checked + "&origemFK=" + origemFK + "&cdSituacao=0&sexo=" + sexo + "&cdTurma=" + turmaFK + "&opcao=0",
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        idProperty: "cd_aluno"
                    }), Memory({ idProperty: "cd_aluno" }));
                    dataStore = new ObjectStore({ objectStore: myStore });
                    var gridAluno = dijit.byId("gridPesquisaAluno");
                    gridAluno.itensSelecionados = [];
                    gridAluno.setStore(dataStore);
                }
                catch (e) {
                    postGerarLog(e);
                }
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#region métodos para  o aluno - pesquisarAlunoFK
function pesquisarAlunoFKAtividadeExtra(pesquisaHabilitada, cursos) {
    try {
        

        var sexo = hasValue(dijit.byId("nm_sexo_fk").value) ? dijit.byId("nm_sexo_fk").value : 0;
        if (pesquisaHabilitada)
            require([
                    "dojo/store/JsonRest",
                    "dojo/data/ObjectStore",
                    "dojo/store/Cache",
                    "dojo/store/Memory"
            ], function (JsonRest, ObjectStore, Cache, Memory) {
                try {
                    var cdsEscolas = (dojo.byId("tagEscola").style.display === "none") ? "" : montarStringCdsEscolas();
                   /* if (!hasValue(cdsEscolas)) {
                        dijit.byId("escolaAlunoFK").set("value", dojo.byId("_ES0").value);
                        dojo.byId("lbEscolaAlunoFk").style.display = "none";
                        require(['dojo/dom-style', 'dijit/registry'],
                            function (domStyle, registry) {

                                domStyle.set(registry.byId("escolaAlunoFK").domNode, 'display', 'none');
                            });
                    } else {
                        dojo.byId("lbEscolaAlunoFk").style.display = "";
                        require(['dojo/dom-style', 'dijit/registry'],
                            function (domStyle, registry) {
                                domStyle.set(registry.byId("escolaAlunoFK").domNode, 'display', '');
                            });
                    }
                    if(dijit.byId("escolaAlunoFK").value == "")
                        dijit.byId("escolaAlunoFK").set("value", dojo.byId("_ES0").value);*/
                    myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/aluno/getAlunoSearchFKPesquisasAtividadeExtra?nome=" + dojo.byId("nomeAlunoFk").value + "&email=" + dojo.byId("emailPesqFK").value + "&status=" + dijit.byId("statusAlunoFK").value + "&cnpjCpf=" + dojo.byId("cpf_fk").value + "&inicio=" +
                            document.getElementById("inicioAlunoFK").checked + "&cdSituacoes=100&sexo=" + sexo + "&semTurma=false&movido=false&tipoAluno=0&cd_pessoa_responsavel=0&cd_escola_combo_fk=0" +"&cursos="+ (cursos || "")+"&cdEscolas=" + cdsEscolas,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        idProperty: "cd_aluno"
                    }), Memory({ idProperty: "cd_aluno" }));
                    dataStore = new ObjectStore({ objectStore: myStore });
                    var gridAluno = dijit.byId("gridPesquisaAluno");
                    gridAluno.itensSelecionados = [];
                    gridAluno.setStore(dataStore);
                }
                catch (e) {
                    postGerarLog(e);
                }
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#endregion
function pesquisaAlunoTurmaFKTurmaFK(origem) {
    try {
        var origemFK = hasValue(origem) ? origem : 0;
        var sexo = hasValue(dijit.byId("nm_sexo_fk").value) ? dijit.byId("nm_sexo_fk").value : 0;
        var myStore =
              dojo.store.Cache(
                      dojo.store.JsonRest({
                          target: Endereco() + "/api/aluno/getAlunoSearchTurma?nome=" + dojo.byId("nomeAlunoFk").value + "&email=" + dojo.byId("emailPesqFK").value + "&status=" + dijit.byId("statusAlunoFK").value + "&cnpjCpf=" + dojo.byId("cpf_fk").value + "&inicio=" + document.getElementById("inicioAlunoFK").checked + "&origemFK=" + origemFK +"&cdSituacao=100&sexo=" + sexo,
                          handleAs: "json",
                          headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                          idProperty: "cd_aluno"
                      }), dojo.store.Memory({ idProperty: "cd_aluno" }));
        dataStore = new dojo.data.ObjectStore({ objectStore: myStore });
        var gridAluno = dijit.byId("gridPesquisaAluno");
        gridAluno.itensSelecionados = [];
        gridAluno.setStore(dataStore);
    }
    catch (e) {
        postGerarLog(e);
    }
}
/*
function findAllEmpresasUsuarioComboFK() {
    require([
        "dojo/_base/xhr",
        "dojo/data/ObjectStore",
        "dojo/store/Memory"
    ], function (xhr, ObjectStore, Memory) {
        xhr.get({
            url: Endereco() + "/api/empresa/findAllEmpresasUsuarioComboFK",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            idProperty: "cd_pessoa"
        }).then(function (data) {
            try {
                showCarregando();

                var dataRetorno = jQuery.parseJSON(data);
                if (hasValue(dataRetorno)) {
                    var dataCombo = dataRetorno.map(function(item, index, array) {
                        return { name: item.dc_reduzido_pessoa, id: item.cd_pessoa +"" };
                    });
                    loadSelect(dataRetorno, "escolaAlunoFK", 'cd_pessoa', 'dc_reduzido_pessoa', dojo.byId("_ES0").value);
                }
                showCarregando();
            }
            catch (e) {
                hideCarregando();
                postGerarLog(e);
            }
        },
            function (error) {
                hideCarregando();
                apresentaMensagem('apresentadorMensagem', error.response.data);
            });
    });
}*/

