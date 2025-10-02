Date.prototype.stdTimezoneOffset = function () {
    var jan = new Date(this.getFullYear(), 0, 1);
    var jul = new Date(this.getFullYear(), 6, 1);
    return Math.max(jan.getTimezoneOffset(), jul.getTimezoneOffset());
}

Date.prototype.dst = function () {
    return this.getTimezoneOffset() < this.stdTimezoneOffset();
}



function login() {
    showCarregando(document.getElementById('login-bg'));

    require(["dojo", "dojo/dom", "dojo/request/xhr", "dojox/json/ref", "dojo/store/Memory"],
        function (dojo, dom, xhr, ref, Memory) {

            var cookieStr = document.cookie;
            var cookieObj = null;
            if (hasValue(cookieStr)) {
	            cookieObj = formataCookie(cookieStr);
            }

            if (verificaCookie(cookieObj)) {
	            xhr.post(Endereco() + "/auth/Login",
		            {
			            headers: { "Accept": "application/json", "Content-Type": "application/json" },
			            handleAs: "json",
			            data: ref.toJson({
				            credentials: {
					            Login: dom.byId("Login").value,
					            Password: dom.byId("Password").value,
					            IdFusoHorario: (new Date().getTimezoneOffset() / 60),
					            IdHorarioVerao: (new Date()).dst()
				            }
			            })
		            }).then(function(data) {
			            if (hasValue(data.retorno)) {
				            if (hasValue(jQuery.parseJSON(data.retorno).Empresas)) {
					            var empresas = jQuery.parseJSON(data.retorno).Empresas;

					            show('divEscolas');
					            show('divSubmit');
					            show('divSubmitEscola');

					            // Desabilita os campos de login e senha:
					            dijit.byId('Password').attr("disabled", true);
					            dijit.byId('Login').attr("disabled", true);

					            var items = new Array();
					            for (var i = 0; i < empresas.length; i++)
						            items.push({ id: empresas[i].cd_pessoa, name: empresas[i].no_pessoa });

					            var stateStore = new Memory({
						            data: items
					            });
					            dijit.byId("cb_escola").store = stateStore;
					            dojo.byId('linkSenhaPerdida').style.display = 'none';

					            //Configura o botão da escola para disparar o enter:
					            dojo.query("#cb_escola").on("keyup",
						            function(e) {
							            if (e.keyCode == 13)
								            marcarEscola();
						            });
				            }
				            if (hasValue(jQuery.parseJSON(data.retorno).id_trocar_senha) &&
					            jQuery.parseJSON(data.retorno).id_trocar_senha) {
					            montarDialog("dlgMudancaSenha", dijit.byId("dlgMudancaSenha"));
					            apresentaMensagem('apresentadorMensagemTrocarSenha', data);
				            }
			            } else if (hasValue(data.MensagensWeb))
				            caixaDialogo(DIALOGO_ERRO, data.MensagensWeb[0].mensagem, 0, 0, 0);
			            else
				            window.location = incluiNoCache('/');
			            showCarregando(document.getElementById('login-bg'));
		            },
		            function(error) {
			            if (hasValue(error) && error.toString().indexOf('SyntaxError:') >= 0)
				            caixaDialogo(DIALOGO_ERRO, msgSessaoExpirada, 0, 0, 0);
			            else
				            caixaDialogo(DIALOGO_ERRO, error, 0, 0, 0);
			            showCarregando(document.getElementById('login-bg'));
		            });
            } else {
	            showCarregando(document.getElementById('login-bg'));
                caixaDialogo(DIALOGO_AVISO, "Para acessar o sistema é necessário aceitar os termos de uso.", 0, 0, 0);
            }
   

           

                
            });
}

function verificaAmbienteTeste(){
    if (document.getElementById("_AMB").value.toLowerCase() == '"true"') {
        document.getElementById("login-bg").style.background = 'url(../images/login/fundo_login_verde.jpg) no-repeat center center fixed';
        document.getElementById("tituloAmbienteTeste").style.display = 'block';
    }
}

function marcarEscola() {
    require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        if (!hasValue(dijit.byId("cb_escola").value))
            caixaDialogo(1, 'Selecione uma escola para efetuar o login.', 0, 0, 0);
        else
            xhr.post(Endereco() + "/auth/postescola", {
                headers: { "Accept": "application/json", "Content-Type": "application/json" },
                handleAs: "json",
                data: ref.toJson({
                    usuario: {
                        no_login: dom.byId("Login").value,
                        dc_senha_usuario: dom.byId("Password").value
                    },
                    Empresas: [{
                        cd_pessoa: dijit.byId("cb_escola").get('value'),
                        no_pessoa: document.getElementById("cb_escola").value
                    }]
                })
            }).then(function (data) {
                if (!hasValue(data.MensagensWeb))
                    window.location = incluiNoCache('/');
                else
                    caixaDialogo(DIALOGO_ERRO, data.MensagensWeb[0].mensagem, 0, 0, 0);
            },
            function (error) {
                if (hasValue(error) && error.toString().indexOf('SyntaxError:') >= 0)
                    caixaDialogo(DIALOGO_ERRO, msgSessaoExpirada, 0, 0, 0);
                else
                    caixaDialogo(DIALOGO_ERRO, error, 0, 0, 0);
            });
    });
}

function montarDialog(nome, e) {
    var offSetX = 20;
    var offSetY = 20;
    $("#" + nome).css("top", e.pageX + offSetX).css("left", e.pageY + offSetY);
    dijit.byId(nome).show();
    dijit.byId(nome).style.display = "block";
    clearForm('formChangePassword');
    dijit.byId('NovaSenha').reset();
    dijit.byId('ConfirmaNovaSenha').reset();
    dijit.byId("des_login").set("value", dijit.byId("Login").value);
    dijit.byId("des_email").value = "";
}

function loginChangePassword() {
    require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref", "dojo/store/Memory"],
            function (dom, xhr, ref, Memory) {

                if (!dijit.byId("formChangePassword").validate()) {
                    return false;
                }
                showCarregando(document.getElementById('login-bg'));
                xhr.post(Endereco() + "/auth/LoginChangePassword", {
                    headers: { "Accept": "application/json", "Content-Type": "application/json" },
                    handleAs: "json",
                    data: ref.toJson({
                        credentials: {
                            Login: dom.byId("Login").value,
                            SenhaAtual: dom.byId("Password").value,
                            NovaSenha: dom.byId("NovaSenha").value,
                            ConfirmaNovaSenha: dom.byId("ConfirmaNovaSenha").value
                        }
                    })
                }).then(function (data) {
                    if (hasValue(data.retorno)) {
                        dijit.byId("dlgMudancaSenha").hide();
                        dijit.byId('Password').set("value", dom.byId("NovaSenha").value);
                        var empresas = jQuery.parseJSON(data.retorno).Empresas;

                        show('divEscolas');
                        show('divSubmit');
                        show('divSubmitEscola');
                        showP('linkSenhaPerdida', false);

                        // Desabilita os campos de login e senha:
                        dijit.byId('Password').attr("disabled", true);
                        dijit.byId('Login').attr("disabled", true);

                        var items = new Array();
                        for (var i = 0; i < empresas.length; i++)
                            items.push({ id: empresas[i].cd_pessoa, name: empresas[i].no_pessoa });

                        var stateStore = new Memory({
                            data: items
                        });
                        dijit.byId("cb_escola").store = stateStore;

                        //Configura o botão da escola para disparar o enter:
                        dojo.query("#cb_escola").on("keyup", function (e) {
                            if (e.keyCode == 13)
                                marcarEscola();
                        });
                    }
                    else if (hasValue(data.MensagensWeb))
                        caixaDialogo(DIALOGO_ERRO, data.MensagensWeb[0].mensagem, 0, 0, 0);
                    else
                        window.location = Endereco() + '/';
                    showCarregando(document.getElementById('login-bg'));
                },
                function (error) {
                    showCarregando(document.getElementById('login-bg'));
                    caixaDialogo(DIALOGO_ERRO, error, 0, 0, 0);
                });
            });
}

function senhaPerdida() {
    apresentaMensagem('apresentadorMensagemSenhaPerdida', null);
    require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref"],
                 function (dom, xhr, ref) {

                     if (!dijit.byId("formSenhaPerdida").validate())
                         return false;
                     showCarregando(document.getElementById('login-bg'));
                     xhr.post(Endereco() + "/auth/trocarSenhaUsuario", {
                         headers: { "Accept": "application/json", "Content-Type": "application/json" },
                         handleAs: "json",
                         data: ref.toJson({
                             sendEmail: {
                                 login: dom.byId("des_login").value,
                                 email: dom.byId("des_email").value
                             }
                         })
                     }).then(function (data) {
                         if (hasValue(data.retorno)) {
                             var mensagensWeb = new Array();
                             mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, data.MensagensWeb[0].mensagem);
                             apresentaMensagem('apresentadorMensagemSenhaPerdida', mensagensWeb);
                         }
                         else {
                             var mensagensWeb = new Array();
                             mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, data.erro.MensagensWeb[0].mensagem);
                             apresentaMensagem('apresentadorMensagemSenhaPerdida', mensagensWeb);
                         }
                         showCarregando(document.getElementById('login-bg'));
                     },
        function (error) {
            showCarregando(document.getElementById('login-bg'));
            caixaDialogo(DIALOGO_ERRO, error, 0, 0, 0);
        });
    });
}