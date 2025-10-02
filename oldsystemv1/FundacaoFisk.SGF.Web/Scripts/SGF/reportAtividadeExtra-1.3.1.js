var TODOS = 0, TODOS_PARTICIPACAO = 2;

function montarMetodosRelatorioAtividadeExtra() {
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
                //loadPeriodo(Memory, registry, ItemFileReadStore);
                montarParticipacao("pesParticipacao", Memory, on)
                montarParticipacao("cbLancada", Memory, on)
                returnDataAtividadeExtraParaPesquisa('pesTipo', 'pesCurso', 'pesProduto');

                new Button({ label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { emitirRelatorio(); } }, "relatorioAtividadeExtra");
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

                decreaseBtn(document.getElementById("pesAluno"), '18px');
                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        dojo.byId("cdAlunoPesq").value = 0;
                        dojo.byId("noAluno").value = null;
                        dijit.byId("noAluno").value = null;
                        apresentaMensagem('apresentadorMensagem', null);
                        dijit.byId("limparAluno").set('disabled', true);
                    }
                }, "limparAluno");
                decreaseBtn(document.getElementById("limparPesAluno"), '40px');

                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            if (!hasValue(dijit.byId('gridPesquisaProfessor')))
                                montarGridPesquisaProfessor(true, function () {
                                    abrirProfessorFK();
                                    dijit.byId("pesquisarProfessorFK").on("click", function (event) {
                                        apresentaMensagem('apresentadorMensagem', null);
                                        pesquisarFuncionarioFK();
                                    });
                                });
                            else abrirProfessorFK();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesFuncionario");
                decreaseBtn(document.getElementById("pesFuncionario"), '18px');
                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        try {
                            dojo.byId("cd_funcionario").value = 0;
                            dojo.byId("cd_pessoa_funcionario").value = 0;
                            dojo.byId("noFuncionario").value = "";
                            dijit.byId("limparPesFuncionario").set('disabled', true);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    },
                }, "limparPesFuncionario");
                decreaseBtn(document.getElementById("limparPesFuncionario"), '40px');
               


                dijit.byId("pesCurso").on("change", function (cdProg) {
                    try {
                        if (!hasValue(cdProg) || cdProg < 0)
                            dijit.byId("pesCurso").set("value", TODOS);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("pesProduto").on("change", function (cdProg) {
                    try {
                        if (!hasValue(cdProg) || cdProg < 0)
                            dijit.byId("pesProduto").set("value", TODOS);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
               
                showCarregando()
                //if (hasValue(dijit.byId("menuManual"))) {
                //dijit.byId("menuManual").on("click", function (e) {
                //    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323076', '765px', '771px');
                //});}
                
            }
            catch (e) {
                postGerarLog(e);
            }
            showCarregando();
        });
    });
}




function abrirProfessorFK() {
    try {
        limparPesquisaProfessorFK();
        pesquisarFuncionarioFK();
        dijit.byId("proPessoaRel").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarProfessorFK() {
    try {
        var valido = true;
        var gridPesquisaProfessor = dijit.byId("gridPesquisaProfessor");
        if (!hasValue(gridPesquisaProfessor.itensSelecionados) || gridPesquisaProfessor.itensSelecionados.length <= 0 || gridPesquisaProfessor.itensSelecionados.length > 1) {
            if (gridPesquisaProfessor.itensSelecionados == null || gridPesquisaProfessor.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridPesquisaProfessor.itensSelecionados != null && gridPesquisaProfessor.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        } else {
            dojo.byId("cd_funcionario").value = gridPesquisaProfessor.itensSelecionados[0].cd_funcionario;
            dojo.byId("cd_pessoa_funcionario").value = gridPesquisaProfessor.itensSelecionados[0].cd_pessoa_funcionario;
            dojo.byId("noFuncionario").value = gridPesquisaProfessor.itensSelecionados[0].dc_reduzido_pessoa;
        }
        if (!valido)
            return false;
        dijit.byId("limparPesFuncionario").set("disabled", false);
        dijit.byId("proPessoaRel").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}



function pesquisarFuncionarioFK() {
    try {
        //Caso tenha horários, verifica se existe horários marcados:
        var trHorarios = document.getElementById('trHorarios');

        if (trHorarios.style.display == 'none' || (trHorarios.style.display != 'none' && dijit.byId('cbHorarios').validate())) {
            var sexo = hasValue(dijit.byId("nm_sexo_prof").value) ? dijit.byId("nm_sexo_prof").value : 0;
            var status = hasValue(dijit.byId("statusProf").value) ? dijit.byId("statusProf").value : 0;
            require([
                "dojo/store/JsonRest",
                "dojo/data/ObjectStore",
                "dojo/store/Cache",
                "dojo/store/Memory"
            ], function (JsonRest, ObjectStore, Cache, Memory) {
                try {
                    var myStore = Cache(
                        JsonRest({
                            target: Endereco() + "/api/funcionario/GetFuncionarioComAtividadeExtraSearch?nome=" + dojo.byId("nomeProf").value + "&apelido=" + dojo.byId("nomeRed").value + "&status=" + parseInt(status) +
                                "&cpf=" + dojo.byId("cpfProf").value + "&inicio=" + document.getElementById("inicioProf").checked + "&tipo=" + TODOS + "&sexo=" + dijit.byId("nm_sexo_prof").value + "&cdAtividade=0",
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                            idProperty: "cd_atividade_extra"
                        }), Memory({ idProperty: "cd_atividade_extra" }));
                  
                    dataStore = new ObjectStore({ objectStore: myStore });
                   
  
                    var gridProfessor = dijit.byId("gridPesquisaProfessor");
                    gridProfessor.itensSelecionados = [];
                    gridProfessor.setStore(dataStore);
                } catch (e) {
                    postGerarLog(e);
                }
            });
        }
    } catch (e) {
        postGerarLog(e);
    }
}




function pesquisaAlunoFKRel() {
    try {
        var sexo = hasValue(dijit.byId("nm_sexo_fk").value) ? dijit.byId("nm_sexo_fk").value : 0;
        
        var myStore = dojo.store.Cache(
            dojo.store.JsonRest({
                target: Endereco() + "/api/aluno/getAlunoPorTurmaSearch?nome=" + dojo.byId("nomeAlunoFk").value + "&email=" + dojo.byId("emailPesqFK").value + "&inicio=" +
                        document.getElementById("inicioAlunoFK").checked + "&origemFK=0" + "&status=" + dijit.byId("statusAlunoFK").value + "&cpf=" + dojo.byId("cpf_fk").value +
                        "&cdSituacao=0&sexo=" + sexo + "&cdTurma=0&opcao=0",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                idProperty: "cd_aluno"
            }), dojo.store.Memory({ idProperty: "cd_aluno" }));

        var dataStore = new dojo.data.ObjectStore({ objectStore: myStore });
        var gridAluno = dijit.byId("gridPesquisaAluno");
        gridAluno.itensSelecionados = [];
        gridAluno.setStore(dataStore);
    } catch (e) {
        postGerarLog(e);
    }
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
            dijit.byId("noAluno").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
            dojo.byId("noAluno").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
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

//function riqueridDatas(bool) {
//    dijit.byId("dtInicialComp").set("required", bool);
//    dijit.byId("dtFinalComp").set("required", bool);
//}



function abrirAlunoFK() {
    try {
        dojo.byId('tipoRetornoAlunoFK').value = RELMATRICULA;
        dijit.byId("proAluno").show();
        limparPesquisaAlunoFK();
        pesquisaAlunoFKRel();
    }
    catch (e) {
        postGerarLog(e);
    }
}



function returnDataAtividadeExtraParaPesquisa(fieldTipoAtividade, fieldCurso, fieldProduto ) {
    require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
        xhr.post(Endereco() + "/api/coordenacao/returnDataAtividadeExtraParaPesquisa", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson(null)
        }).then(function (dataAtividadeExtra) {
            try {
                dataAtividadeExtra = jQuery.parseJSON(dataAtividadeExtra);
                if (!hasValue(dataAtividadeExtra.erro)) {
                    loadTipoAtividadeExtra(dataAtividadeExtra.retorno.tiposAtividadeExtras, fieldTipoAtividade);
                    loadProduto(dataAtividadeExtra.retorno.produtos, fieldProduto);
                    loadCurso(dataAtividadeExtra.retorno.cursos, fieldCurso);
                } else
                    apresentaMensagem('apresentadorMensagem', dataAtividadeExtra);
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



function emitirRelatorio() {
    try {
        if (hasValue(dojo.byId("dtInicial").value && hasValue(dojo.byId("dtFinal").value))) {
            this.tipoAtividade = hasValue(dijit.byId("pesTipo").value) ? dijit.byId("pesTipo").value : null;
            this.produto = hasValue(dijit.byId("pesProduto").value) ? dijit.byId("pesProduto").value : null;
            this.curso = hasValue(dijit.byId("pesCurso").value) ? dijit.byId("pesCurso").value : null;
            this.aluno = hasValue(dijit.byId("noAluno").cd_pessoa)
                ? dijit.byId("noAluno").cd_pessoa
                : (hasValue(dijit.byId("noAluno").value) ? dijit.byId("noAluno").value : null);
            this.responsavel =  hasValue(dojo.byId("cd_funcionario").value) ? dojo.byId("cd_funcionario").value : 0;
            this.participacao = hasValue(dijit.byId("pesParticipacao").value)
                ? dijit.byId("pesParticipacao").value
                : null;
            this.escondeObs = dojo.byId("ckObsAtividade").checked ? true : false;

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
            this.lancada = hasValue(dijit.byId("cbLancada").value)
                ? dijit.byId("cbLancada").value
                : null;
            var AtividadeExtra = function (tipoAtividade,
                produto,
                curso,
                responsavel,
                aluno,
                participacao,
                escondeObs,
                dataInicial,
                dataFinal,
                lancada) {
                this.tipoAtividade = tipoAtividade;
                this.produto = produto;
                this.curso = curso;
                this.responsavel = responsavel;
                this.aluno = aluno;
                this.participacao = participacao;
                this.escondeObs = escondeObs;
                this.dataInicial = dataInicial;
                this.dataFinal = dataFinal;
                this.lancada = lancada;
            };

            /*AtividadeExtra*/
            atividadeExtra = new AtividadeExtra(tipoAtividade,
                produto,
                curso,
                responsavel,
                aluno,
                participacao,
                escondeObs,
                dataInicial,
                dataFinal,
                lancada);

            if (!testaPeriodoAtividadeExtra(atividadeExtra.dataInicial, atividadeExtra.dataFinal)) {
                return false;
            } //Valida data Final
            dojo.xhr.get({
                url: Endereco() + "/api/secretaria/GetUrlRelatorioAtividadeExtra",
                preventCache: true,
                handleAs: "json",
                content: {
                    sort: "",
                    cd_atividade_extra: atividadeExtra.tipoAtividade,
                    cd_produto: atividadeExtra.produto,
                    cd_curso: atividadeExtra.curso,
                    cd_aluno: atividadeExtra.aluno,
                    cd_funcionario: atividadeExtra.responsavel,
                    id_participacao: atividadeExtra.participacao,
                    esconde_obs: atividadeExtra.escondeObs,
                    dta_ini: atividadeExtra.dataInicial,
                    dta_fim: atividadeExtra.dataFinal,
                    id_lancada: atividadeExtra.lancada
                },
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function(data) {
                    abrePopUp(Endereco() + '/Relatorio/ReportGestaoAtividadeExtra?' + data,
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


//function loadPeriodo(Memory, registry, ItemFileReadStore) {
//    try {
//        var w = registry.byId("cbPeriodo");
//        var stateStore = [
//                   { name: "Manhã", id: "1" },
//                   { name: "Tarde", id: "2" },
//                   { name: "Noite", id: "3" }
//        ];
//        var store = new ItemFileReadStore({
//            data: {
//                identifier: "id",
//                label: "name",
//                items: stateStore
//            }
//        });
//        w.setStore(store, []);
//    }
//    catch (e) {
//        postGerarLog(e);
//    }
//}


function loadTipoAtividadeExtra(items, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
  function (Memory, Array) {
      try {
          var itemsCb = [];
          var cbTipoAtividade = dijit.byId(field);
          Array.forEach(items, function (value, i) {
              itemsCb.push({ id: value.cd_tipo_atividade_extra, name: value.no_tipo_atividade_extra });
          });
          var stateStore = new Memory({
              data: itemsCb
          });
          cbTipoAtividade.store = stateStore;
      }
      catch (e) {
          postGerarLog(e);
      }
  });
}
//fim  Métodos Tipo Atividade


function loadCurso(items, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try {
            var itemsCb = [];
            var cbCurso = dijit.byId(field);
            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_curso, name: value.no_curso });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbCurso.store = stateStore;
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}


function loadProduto(items, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try {
            var itemsCb = [];
            var cbProduto = dijit.byId(field);
            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_produto, name: value.no_produto });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbProduto.store = stateStore;
        }
        catch (e) {
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



