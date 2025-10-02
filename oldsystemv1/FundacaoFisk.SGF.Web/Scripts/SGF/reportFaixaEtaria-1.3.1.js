
function montarMetodosRelatorioFaixaEtaria() {
    require([
    "dojo/ready",
    "dojo/_base/xhr",
    "dojo/store/Memory",
    "dojo/on",
    "dijit/form/Button",
    "dojo/store/JsonRest",
    "dojo/data/ObjectStore",
    "dojo/store/Cache",
    "dijit/Tooltip",
    "dojo/on",
    "dijit/form/FilteringSelect",
    "dijit/registry",
    "dojo/data/ItemFileReadStore"
    ], function (ready, xhr, Memory, on, Button, JsonRest, ObjectStore, Cache, Tooltip, on, FilteringSelect, registry, ItemFileReadStore) {
        ready(function () {
            try {
                showCarregando();
                montarOpcao("cbOpcao", null);

                new Button({ label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { emitirRelatorio(); } }, "relatorioFaixaEtaria");

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

                dijit.byId('idadeInicial').on('change',
                    function (e) {
                        if (hasValue(dijit.byId('idadeFinal')) &&
                            hasValue(dijit.byId('idadeInicial')) &&
                            dijit.byId('idadeInicial').value > dijit.byId('idadeFinal').value) {
                            var mensagensWeb = [];
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO,
                                msgErroIdadeFinalMenorIdadeInicial);
                            apresentaMensagem('apresentadorMensagem', mensagensWeb);
                        } else {
                            apresentaMensagem('apresentadorMensagem', null);
                        }
                    });

                dijit.byId('idadeFinal').on('change',
                    function (e) {
                        if (hasValue(dijit.byId('idadeFinal')) &&
                            hasValue(dijit.byId('idadeInicial')) &&
                            dijit.byId('idadeInicial').value > dijit.byId('idadeFinal').value) {
                            var mensagensWeb = [];
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO,
                                msgErroIdadeFinalMenorIdadeInicial);
                            apresentaMensagem('apresentadorMensagem', mensagensWeb);
                        } else {
                            apresentaMensagem('apresentadorMensagem', null);
                        }
                    });

            }
            catch (e) {
                showCarregando();
                postGerarLog(e);
            }
        });
    });
}

function validaIdade() {
    try {
        if (hasValue(dijit.byId('idadeFinal')) &&
            hasValue(dijit.byId('idadeInicial')) &&
            dijit.byId('idadeInicial').value > dijit.byId('idadeFinal').value) {
            var mensagensWeb = [];
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO,
                msgErroIdadeFinalMenorIdadeInicial);
            apresentaMensagem('apresentadorMensagem', mensagensWeb);
            return false;
        } else {
            return true;
        }
    } catch (e) {
        postGerarLog(e);
    } 
}

function emitirRelatorio() {
    try {

        if (!validaIdade()) {
            return false;
        }

        this.tipo = hasValue(dijit.byId("cbOpcao").value) ? dijit.byId("cbOpcao").value : 0;
        this.idade = hasValue(dijit.byId("idadeInicial").value) ? dijit.byId("idadeInicial").value : 1;
        this.idade_max = hasValue(dijit.byId("idadeFinal").value) ? dijit.byId("idadeFinal").value : 99;
        this.cd_turma = (dojo.byId('cdTurmaPes').value != null) ? dojo.byId('cdTurmaPes').value : 0;
        this.no_turma = hasValue(dojo.byId("noTurma").value) ? dojo.byId("noTurma").value : "";

            var FaixaEtaria = function (tipo, idade, idade_max, cd_turma, no_turma) {
                this.tipo = tipo;
                this.idade = idade;
                this.idade_max = idade_max;
                this.cd_turma = cd_turma;
                this.no_turma = no_turma;

            };

            /*Faixa Etaria*/
            faixaEtaria = new FaixaEtaria(this.tipo, this.idade, this.idade_max, this.cd_turma, this.no_turma);


            dojo.xhr.get({
                url: Endereco() + "/api/secretaria/GeturlrelatorioFaixaEtaria",
                preventCache: true,
                handleAs: "json",
                content: {
                    sort: "",
                    tipo: faixaEtaria.tipo,
                    idade: faixaEtaria.idade,
                    idade_max: faixaEtaria.idade_max,
                    cd_turma: faixaEtaria.cd_turma,
                    no_turma: faixaEtaria.no_turma
                },
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                abrePopUp(Endereco() + '/Relatorio/RelatorioFaixaEtaria?' + data,
                    '1024px',
                    '750px',
                    'popRelatorio');
            },
                function (error) {
                    apresentaMensagem('apresentadorMensagem', error);
                });
        

    }
    catch (e) {
        postGerarLog(e);
    }
}

function showMessage(el, tipoErro, msg) {
    var mensagensWeb = new Array();
    apresentaMensagem(el, null);
    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msg);
    apresentaMensagem(el, mensagensWeb);
}



function montarOpcao(nomElement, tipo) {
    require(["dojo/store/Memory"],
        function (Memory) {
            try {
                var dados = [
                    { name: "Todas", id: "0" },
                    { name: "Alunos Ativos", id: "1" },
                    { name: "Alunos Desistentes", id: "2" },
                    { name: "ExAlunos", id: "3" },
                    { name: "Clientes", id: "4" },
                    { name: "Funcionários", id: "5" },
                    { name: "Professor", id: "6" }];

                var statusStore = new Memory({
                    data: dados
                });
                dijit.byId(nomElement).store = statusStore;
                dijit.byId(nomElement).set("value", 0);
            } catch (e) {
                postGerarLog(e);
            }
        });
};


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





