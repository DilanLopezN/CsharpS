var TODOS = 0, TODOS_PARTICIPACAO = 2, AULAREPOSICAO = 28;

function montarMetodosRelatorioAulaReposicao() {
    require([
    "dojo/ready",
    "dojo/_base/xhr",
    "dojo/store/Memory",
    "dojo/on",
    "dijit/form/Button",
    "dojo/store/JsonRest",
    "dojo/data/ObjectStore",
    "dojo/store/Cache",
    "dojo/on",
    "dijit/form/FilteringSelect",
    "dijit/registry",
    "dojo/data/ItemFileReadStore"
    ], function (ready, xhr, Memory, on, Button, JsonRest, ObjectStore, Cache, on, FilteringSelect, registry, ItemFileReadStore) {
        ready(function () {
            showCarregando();
            try {
                sugereDataCorrente();
                montarParticipacao("cb_participacao", Memory, on)
                returnDataAulaReposicaoParaPesquisa('cbResponsavel', 'cbAluno');

                new Button({ label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { emitirRelatorio(); } }, "relatorioAulaReposicao");
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                montarGridPesquisaAluno(false, function () {
                                    abrirAlunoFK();
                                });
                            }
                            else
                                abrirAlunoFK();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesAluno");
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaTurmaFK"))) {
                                montarGridPesquisaTurmaFK(function () {
                                    abrirTurmaFK();
                                    dijit.byId("pesAlunoTurmaFK").on("click", function (e) {
                                        if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                            montarGridPesquisaAluno(false, function () {
                                                abrirAlunoFKTurmaFK(true, AULAREPOSICAO);
                                            });
                                        }
                                        else
                                            abrirAlunoFKTurmaFK(true, AULAREPOSICAO);
                                    });
                                });
                            }
                            else
                                abrirTurmaFK();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesTurma");

                decreaseBtn(document.getElementById("pesAluno"), '18px');
                decreaseBtn(document.getElementById("pesTurma"), '18px');

                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        dojo.byId("cdAluno").value = 0;
                        dojo.byId("edAluno").value = null;
                        dijit.byId("edAluno").value = null;
                        apresentaMensagem('apresentadorMensagem', null);
                        dijit.byId("limparAluno").set('disabled', true);
                    }
                }, "limparAluno");
                decreaseBtn(document.getElementById("limparAluno"), '40px');
              
                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        dojo.byId("cdTurma").value = 0;
                        dojo.byId("edTurma").value = null;
                        dijit.byId("edTurma").value = null;
                        apresentaMensagem('apresentadorMensagem', null);
                        dijit.byId("limparTurma").set('disabled', true);
                    }
                }, "limparTurma");
                decreaseBtn(document.getElementById("limparTurma"), '40px');

                showCarregando()
                
            }
            catch (e) {
                postGerarLog(e);
            }
            showCarregando();
        });
    });
}

function getReturnDataAulaReposicao(fieldProfessor, fieldAluno) {
    var aulaReposicaoPesq = new AulaReposicaoPesquisa(null);
    require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
        xhr.post(Endereco() + "/coordenacao/returnDataAulaReposicao", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson(aulaReposicaoPesq)
        }).then(function (dataAulaReposicao) {
            try {
                if (!hasValue(dataAulaReposicao.erro)) {
                    loadProfessor(dataAulaReposicao.retorno.professores, fieldProfessor);
                } else
                    apresentaMensagem('apresentadorMensagem', dataAulaReposicao);
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error.response.data);
        });
    });
}

function loadProfessor(items, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try {
            var itemsCb = [];
            var cbResponsavel = dijit.byId(field);
            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_pessoa, name: value.no_pessoa });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbResponsavel.store = stateStore;
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

//function pesquisaAlunoFKRel(origem) {
//    try {
//        var sexo = hasValue(dijit.byId("nm_sexo_fk").value) ? dijit.byId("nm_sexo_fk").value : 0;
//        var origemFK = hasValue(origem) ? origem : 0;
//        var myStore = dojo.store.Cache(
//            dojo.store.JsonRest({
//                target: Endereco() + "/api/aluno/getAlunoPorTurmaSearch?nome=" + dojo.byId("nome").value + "&email=" + dojo.byId("emailPesqFK").value + "&inicio=" +
//                        document.getElementById("inicioAlunoFK").checked + "&origemFK=" + origemFK + "&status=" + dijit.byId("statusAlunoFK").value + "&cpf=" + dojo.byId("cpf_fk").value +
//                        "&cdSituacao=0&sexo=" + sexo + "&cdTurma=0&opcao=0",
//                handleAs: "json",
//                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
//                idProperty: "cd_aluno"
//            }), dojo.store.Memory({ idProperty: "cd_aluno" }));

//        var dataStore = new dojo.data.ObjectStore({ objectStore: myStore });
//        var gridAluno = dijit.byId("gridPesquisaAluno");
//        gridAluno.itensSelecionados = [];
//        gridAluno.setStore(dataStore);
//    } catch (e) {
//        postGerarLog(e);
//    }
//}

//function pesquisaTurmaFKRel(origem) {
//    try {
//        var origemFK = hasValue(origem) ? origem : 0;
//        var myStore = dojo.store.Cache(
//            dojo.store.JsonRest({
//                target: Endereco() + "/api/aluno/getAlunoPorTurmaSearch?nome=" + dojo.byId("nome").value + "&email=" + dojo.byId("emailPesqFK").value + "&inicio=" +
//                        document.getElementById("inicioAlunoFK").checked + "&origemFK=" + origemFK + "&status=" + dijit.byId("statusAlunoFK").value + "&cpf=" + dojo.byId("cpf_fk").value +
//                        "&cdSituacao=0&sexo=" + sexo + "&cdTurma=0&opcao=0",
//                handleAs: "json",
//                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
//                idProperty: "cd_turma"
//            }), dojo.store.Memory({ idProperty: "cd_turma" }));

//        var dataStore = new dojo.data.ObjectStore({ objectStore: myStore });
//        var gridTurma = dijit.byId("gridPesquisaTurmaFK");
//        gridTurma.itensSelecionados = [];
//        gridTurma.setStore(dataStore);
//    } catch (e) {
//        postGerarLog(e);
//    }
//}

function retornarAlunoFKAtivdadeExtra() {
    retornarAlunoRelFK()
}

function retornarAlunoRelFK() {
    try {
        var valido = true;
        var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");
        if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0 || gridPesquisaAluno.itensSelecionados.length > 1) {
            if (gridPesquisaAluno.itensSelecionados != null && gridPesquisaAluno.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);

            if (gridPesquisaAluno.itensSelecionados != null && gridPesquisaAluno.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            dojo.byId("cdAluno").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
            dojo.byId("edAluno").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
            dijit.byId('limparAluno').set("disabled", false);
        }
        if (!valido)
            return false;
        dijit.byId("proAluno").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarTurmaFKAulaReposicao() {
    try {
        if (hasValue(dojo.byId("idOrigemCadastro").value) && dojo.byId("idOrigemCadastro").value == AULAREPOSICAO) {
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
                dojo.byId("cdTurma").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
                dojo.byId("edTurma").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
                dijit.byId('limparTurma').set("disabled", false);
            }
            if (!valido)
                return false;
            dijit.byId("proTurmaFK").hide();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirAlunoFK() {
    try {
        dojo.byId('tipoRetornoAlunoFK').value = AULAREPOSICAO;
        dijit.byId("gridPesquisaAluno").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
        limparPesquisaAlunoFK();
        pesquisarAlunoFK(true, AULAREPOSICAO);
        dijit.byId("proAluno").show();
        dijit.byId('gridPesquisaAluno').update();
    }
    catch (e) {
        postGerarLog(e);
    }
}


function abrirTurmaFK() {
//    try {
        dojo.byId("trAluno").style.display = "";
        limparFiltrosTurmaFK();
        //Configuração retorno fk de turma
        dojo.byId("idOrigemCadastro").value = AULAREPOSICAO;
        dijit.byId("tipoTurmaFK").store.remove(0);
        if (hasValue(dijit.byId('tipoTurmaFK')))
            dijit.byId('tipoTurmaFK').set('value', 1);
        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        gridPesquisaTurmaFK.setStore(null);
        pesquisarTurmaFK(-1, AULAREPOSICAO);
        dijit.byId("proTurmaFK").show();
        //dojo.byId("legendTurmaFK").style.visibility = "hidden";
//    }
//    catch (e) {
//        postGerarLog(e);
//    }
}

//Método para pesquisa
function returnDataAulaReposicaoParaPesquisa(fieldProfessor, fieldAluno) {
    require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
        xhr.post(Endereco() + "/api/coordenacao/returnDataAulaReposicaoParaPesquisa", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson(null)
        }).then(function (dataAulaReposicao) {
            try {
                dataAulaReposicao = jQuery.parseJSON(dataAulaReposicao);
                if (!hasValue(dataAulaReposicao.erro)) {
                    loadProfessor(dataAulaReposicao.retorno.professores, fieldProfessor);
                    //loadAluno(dataAulaReposicao.retorno.alunos, fieldAluno);
                } else
                    apresentaMensagem('apresentadorMensagem', dataAulaReposicao);
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {

            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, msgLockPermissionSearchFilter);

            apresentaMensagem('apresentadorMensagem', mensagensWeb);
            //apresentaMensagem('apresentadorMensagem', error.response.data);
        });
    });

}

function emitirRelatorio() {
    try {
        if (hasValue(dojo.byId("dtInicial").value && hasValue(dojo.byId("dtFinal").value))) {
            this.turma = hasValue(dojo.byId("cdTurma").value) ? dojo.byId("cdTurma").value : null;
            this.aluno = hasValue(dijit.byId("edAluno").cd_pessoa)
                ? dijit.byId("edAluno").cd_pessoa
                : (hasValue(dojo.byId("cdAluno").value) ? dojo.byId("cdAluno").value : null);
            this.funcionario =  hasValue(dijit.byId("cbResponsavel").value) ? dijit.byId("cbResponsavel").value : 0;
            this.participacao = hasValue(dijit.byId("cb_participacao").value)
                ? dijit.byId("cb_participacao").value
                : null;
            this.escondeObs = dojo.byId("ckObs").checked ? true : false;

            this.dataInicial = dojo.date.locale.parse(dojo.byId("dtInicial").value,
                { formatLength: 'short', selector: 'date', locale: 'pt-br' });
            this.dataInicial = dojo.date.locale.format(this.dataInicial,
                { selector: "date", datePattern: "MM/dd/yyyy", formatLength: "short", locale: "pt-br" });

            if (hasValue(dojo.byId("dtFinal").value)) {
                this.dataFinal = dojo.date.locale.parse(dojo.byId("dtFinal").value,
                    { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                this.dataFinal = dojo.date.locale.format(this.dataFinal,
                    { selector: "date", datePattern: "MM/dd/yyyy", formatLength: "short", locale: "pt-br" });
            } else {
                this.dataFinal = null;
            }
            var AulaReposicao = function(
                turma,
                funcionario,
                aluno,
                participacao,
                escondeObs,
                dataInicial,
                dataFinal) {
                this.turma = turma;
                this.funcionario = funcionario;
                this.aluno = aluno;
                this.participacao = participacao;
                this.escondeObs = escondeObs;
                this.dataInicial = dataInicial;
                this.dataFinal = dataFinal;

            };

            /*AulaReposicao*/
            aulaReposicao = new AulaReposicao(
                turma,
                funcionario,
                aluno,
                participacao,
                escondeObs,
                dataInicial,
                dataFinal);

            if (!testaPeriodoAtividadeExtra(aulaReposicao.dataInicial, aulaReposicao.dataFinal)) {
                return false;
            } //Valida data Final
            dojo.xhr.get({
                url: Endereco() + "/api/secretaria/GetUrlRelatorioAulaReposicao",
                preventCache: true,
                handleAs: "json",
                content: {
                    sort: "",
                    cd_turma: aulaReposicao.turma,
                    cd_aluno: aulaReposicao.aluno,
                    cd_funcionario: aulaReposicao.funcionario,
                    id_participacao: aulaReposicao.participacao,
                    esconde_obs: aulaReposicao.escondeObs,
                    dta_ini: aulaReposicao.dataInicial,
                    dta_fim: aulaReposicao.dataFinal
                },
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function(data) {
                    abrePopUp(Endereco() + '/Relatorio/RelatorioAulaReposicao?' + data,
                        '1024px',
                        '750px',
                        'popRelatorio');
                },
                function(error) {
                    apresentaMensagem('apresentadorMensagem', error);
                });
        } else {
            return;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function createDate(stringDate) {
    var dateParts = stringDate.split("/");

    // month is 0-based, that's why we need dataParts[1] - 1
    var dateObject = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);

    return dateObject;
}

function testaPeriodoAtividadeExtra(dataInicial, dataFinal) {
    try {
        var retorno = true;
       
        var dtInicial = createDate(dataInicial);
        var dtFinal = createDate(dataFinal);

        if (dojo.date.compare(dtInicial, dtFinal) > 0) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataInicialFinal);
            apresentaMensagem('apresentadorMensagem', mensagensWeb);
            retorno = false;
        } else
            apresentaMensagem('apresentadorMensagem', "");
        return retorno;
    } catch (e) {
        postGerarLog(e);
    }
}
//#endregion

function sugereDataCorrente() {
    dojo.xhr.post({
        url: Endereco() + "/util/PostDataHoraCorrente",
        preventCache: true,
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            var dataCorrente = jQuery.parseJSON(data).retorno;
            var dataSugerida = dataCorrente.dataPortugues.split(" ");
            if (dataSugerida.length > 0) {
                var date = dojo.date.locale.parse(dataSugerida[0], { formatLength: 'short', selector: 'date', locale: 'pt-br' });

                //Data Inicial: Um mês antes à data do dia
                dijit.byId('dtInicial').attr("value", new Date(date.getFullYear(), (date.getMonth() - 1), date.getDate()));
                //Data Final: Data do dia
                dijit.byId('dtFinal').attr("value", date);
            }
        } catch (e) {
            postGerarLog(e);
        }
    });
}


function montarParticipacao(nomElement, Memory, on) {

    var dados = [
        { name: "Não", id: "0" },
        { name: "Sim", id: "1" },
        { name: "Todos", id: "2" }
    ]
    var statusStore = new Memory({
        data: dados
    });
    dijit.byId(nomElement).store = statusStore;
    dijit.byId(nomElement).set("value", TODOS_PARTICIPACAO);
    
};
