var TODOS = 0;
var SAIDA = 2;

var EnumsAlunosSemTituloGerado = {
    SITUACAO: 3
}


function montarMeses(idComponente, Memory, on) {

    var dados = [{ name: "Janeiro", id: "1" },
        { name: "Fevereiro", id: "2" },
        { name: "Março", id: "3" },
        { name: "Abril", id: "4" },
        { name: "Maio", id: "5" },
        { name: "Junho", id: "6" },
        { name: "Julho", id: "7" },
        { name: "Agosto", id: "8" },
        { name: "Setembro", id: "9" },
        { name: "Outubro", id: "10" },
        { name: "Novembro", id: "11" },
        { name: "Dezembro", id: "12" }
    ]
    var statusStore = new Memory({
        data: dados
    });
    dijit.byId(idComponente).store = statusStore;
    setarMesAno(idComponente, Memory, on);
    //dijit.byId(idComponente).set("value", 1);
};

function setarMesAno(idComponente, Memory, on) {
    dijit.byId("ano")._onChangeActive = false;
    dijit.byId("pesMes")._onChangeActive = false;
    var data = new Date();
    var ano = data.getFullYear();
    var mes = data.getMonth() + 1;

    
    dijit.byId('ano').set('value', ano);
    dijit.byId('pesMes').set('value', mes);

    dijit.byId("ano")._onChangeActive = true;
    dijit.byId("pesMes")._onChangeActive = true;
}
function loadSituacaoAluno(Memory, registry, ItemFileReadStore) {
    var w = registry.byId('cbSituacao');
    var dados = [
        { name: "Matriculado", id: "1" },
        { name: "Rematriculado", id: "8" },
        { name: "Desistente", id: "2" },
        { name: "Encerrado", id: "4" },
        { name: "Movido", id: "0" }
    ];
    var storeTipoAluno = new ItemFileReadStore({
        data: {
            identifier: "id",
            label: "name",
            items: dados
        }
    });
    w.setStore(storeTipoAluno, []);
   // w.options[0].selected = true;
   // w.options[1].selected = true;
}

function montarAlunosSemTituloGerado() {
    require([
"dojo/ready",
"dojo/_base/xhr",
"dojo/store/Memory",
"dojo/on",
"dijit/form/Button",
"dijit/registry",
"dojo/data/ItemFileReadStore",
"dojo/window"
    ], function (ready, xhr, Memory, on, Button, registry, ItemFileReadStore, window) {
        ready(function () {

            montarMeses('pesMes', Memory, on);
            loadSituacaoAluno(Memory, registry, ItemFileReadStore);
            setMenssageMultiSelect(EnumsAlunosSemTituloGerado.SITUACAO, 'cbSituacao');
            dijit.byId("cbSituacao").on("change", function (e) {
                setMenssageMultiSelectOpcao(EnumsAlunosSemTituloGerado.SITUACAO, 'cbSituacao', true, 0);
            });

            new Button({ label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { emitirRelatorio(window); } }, "rptAlunosSemTituloGerado");
            showCarregando();

            // Partial Turma
            new Button({
                label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                    if (!hasValue(dijit.byId("gridPesquisaTurmaFK"), true))
                        montarGridPesquisaTurmaFK(function () {
                            abrirTurmaFK();
                            dijit.byId("pesAlunoTurmaFK").on("click", function (e) {
                                if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                    montarGridPesquisaAluno(false, function () {
                                        abrirAlunoFKTurmaFK(true);
                                    });
                                }
                                else
                                    abrirAlunoFKTurmaFK(true);
                            });
                        });
                    else
                        abrirTurmaFK();
                }
            }, "pesTurma");

            new Button({
                label: "Limpar", iconClass: '', type: "reset", disabled: true,
                onClick: function () {
                    dojo.byId("noTurma").value = "";
                    dojo.byId("cdTurmaPes").value = 0;
                    dijit.byId("limparTurma").set('disabled', true);
                    apresentaMensagem('apresentadorMensagem', null);
                }
            }, "limparTurma");

            decreaseBtn(document.getElementById("pesTurma"), '18px');
            decreaseBtn(document.getElementById("limparTurma"), '40px');


        });
    });
}


function montarparametrosmulti(componente) {
    var produtos = "";
    if (!hasValue(dijit.byId(componente).value) || dijit.byId(componente).value.length <= 0) {
        produtos = "100";
    } else {

        if (dijit.byId(componente).value[0] == "0") {
            produtos = "0"
        } else {
            for (var i = 0; i < dijit.byId(componente).value.length; i++) {
                if (produtos == "") {
                    produtos = dijit.byId(componente).value[i];
                } else {
                    produtos = produtos + "|" + dijit.byId(componente).value[i];
                }
            }
        }
    }

    return produtos;
}

function emitirRelatorio(windowUtils) {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!dijit.byId("formAlunosSemTituloGerado").validate())
            return false;

        this.no_mes = (hasValue(dijit.byId("pesMes").item)) ? dijit.byId("pesMes").item.name : null;
        this.vl_mes = (dijit.byId("pesMes").value != null && dijit.byId("pesMes").value != undefined) ? dijit.byId("pesMes").value : 0;
        this.ano = (dijit.byId("ano").value != null && dijit.byId("ano").value != undefined) ? dijit.byId("ano").value : 0;
        this.cd_turma = (dojo.byId('cdTurmaPes').value != null) ? dojo.byId('cdTurmaPes').value : 0;
        this.situacoes = montarparametrosmulti('cbSituacao');
        

        var AlunosSemTituloGeradoParams = function (no_mes, vl_mes, ano, cd_turma, situacoes) {
            this.no_mes = no_mes;
            this.vl_mes = vl_mes;
            this.ano = ano;
            this.cd_turma = cd_turma;
            this.situacoes = situacoes;
        };

        /*Objeto parametros relatório*/
        var params = new AlunosSemTituloGeradoParams(no_mes, vl_mes, ano, cd_turma, situacoes);


        dojo.xhr.get({
            url: Endereco() + "/api/aluno/GetUrlRptAlunosSemTituloGerado",
            preventCache: true,
            handleAs: "json",
            content: {
                sort: "",
                no_mes: params.no_mes,
                vl_mes: params.vl_mes,
                ano: params.ano,
                cd_turma: params.cd_turma,
                situacoes: params.situacoes
            },
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            abrePopUp(Endereco() + '/Relatorio/RelatorioAlunosSemTituloGerado?' + data, '1024px', '750px', 'popRelatorio');
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}


function abrirTurmaFK() {
    try {
        //Configuração retorno fk de turma
        dojo.byId("idOrigemCadastro").value = RELMATRICULA;
        dijit.byId("proTurmaFK").show();
        limparFiltrosTurmaFK();
        dijit.byId("tipoTurmaFK").store.remove(0);
        if (hasValue(dijit.byId('tipoTurmaFK'))) {
            dijit.byId('tipoTurmaFK').set('value', 1);
            if (hasValue(dijit.byId("pesProfessorFK").value)) {
                dijit.byId('pesProfessorFK').set('disabled', true);
            } else {
                dijit.byId('pesProfessorFK').set('disabled', false);
            }
        }
        apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
        limparFiltrosTurmaFK();
        dojo.byId("trAluno").style.display = "";
        dojo.byId("idOrigemCadastro").value = RELMATRICULA;
        dijit.byId("proTurmaFK").show();
        pesquisarTurmaFK();

    } catch (e) {
        postGerarLog(e);
    }
}
function retornarTurmaFKRelMatricula() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        var valido = true;
        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        if (!hasValue(gridPesquisaTurmaFK.itensSelecionados) || gridPesquisaTurmaFK.itensSelecionados.length <= 0 || gridPesquisaTurmaFK.itensSelecionados.length > 1) {
            if (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            dojo.byId("noTurma").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
            dojo.byId("cdTurmaPes").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
            dijit.byId('limparTurma').set("disabled", false);
        }
        if (!valido)
            return false;
        dijit.byId("proTurmaFK").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

