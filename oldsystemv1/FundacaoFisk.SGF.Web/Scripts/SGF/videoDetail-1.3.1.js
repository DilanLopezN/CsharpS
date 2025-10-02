
function reproduzirVideo(caminhoVideo) {
	require([
	"dojo/_base/window",
	"dojo/ready",
	"dojox/mobile/Video",
	"dojox/mobile/parser",
	"dojox/mobile",
	 "dojo/dom-attr",
	 "dojo/dom",
	 "dojo/domReady!",
	], function (win, ready, Video, domAttr, dom) {
		ready(function () {
			var widget = Video({
				width: "1127px",
				height: "634px",
				autoplay: true,
				controls: true,
				source: [{ src: caminhoVideo, type: "video/mp4" },
						 { src: caminhoVideo, type: "video/ogg" },
						 { src: caminhoVideo, type: "video/webm" }
				//source: [{ src: "/Content/Uploads/Arquivos/Videos/EspetaculodaNatureza.mp4", type: "video/mp4" }
						 //{ src: "video/sample.ogv", type: "video/ogg" },
						 //{ src: "video/sample.webm", type: "video/webm" }
				]
			});
			//win.body().appendChild(widget.domNode);
			//dom.byId("playerVideo").appendChild(widget.domNode);

			dojo.byId("playerVideo").appendChild(widget.domNode);
			widget.startup();
		});
	});
}


function montarMetodosVideo() {

	//Criação da Grade de sala
	require([
		"dojo/_base/xhr",
		"dijit/registry",
		"dojox/grid/EnhancedGrid",
		"dojox/grid/DataGrid",
		"dojox/grid/enhanced/plugins/Pagination",
		"dojo/store/JsonRest",
		"dojox/data/JsonRestStore",
		"dojo/data/ObjectStore",
		"dojo/data/ItemFileReadStore",
		"dojo/store/Cache",
		"dojo/store/Memory",
		"dojo/query",
		"dojo/dom-attr",
		"dijit/form/Button",
		"dijit/form/TextBox",
		"dojo/ready",
		"dijit/form/DropDownButton",
		"dijit/DropDownMenu",
		"dijit/MenuItem",
		"dojo/parser",
		"dojo/dom",
		"dojo/on",
		"dojo/has",
		"dijit/form/DateTextBox",
		"dijit/Dialog",
		"dojo/parser",
		"dojo/domReady!",
		"dijit/Tooltip",
		"dojox/form/Uploader",
		"dojox/form/uploader/FileList",
		"dojox/form/uploader/plugins/HTML5",
		"dojox/form/FileUploader",
		"dojo/aspect"
	], function (xhr, registry, EnhancedGrid, DataGrid, Pagination, JsonRest, JsonRestStore, ObjectStore, ItemFileReadStore, Cache, Memory, query, domAttr, Button, TextBox, ready, DropDownButton, DropDownMenu, MenuItem, parser, dom, on, has, DateTextBox, Tooltip, Uploader, FileList, HTML5, FileUploader, aspect) {
		ready(function () {
			showCarregando();

			try {
				

				//marca true e desabilita se veio do popup de prospects não lidos
				var urlParams = new URLSearchParams(window.location.search);
				var cd_video = urlParams.get("cd_video");
				
				

				xhr.get({
					url: Endereco() + "/api/coordenacao/obterVideoPorID",
					preventCache: true,
					content: {
						cd_video: cd_video
					},
					handleAs: "json",
					headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
				}).then(function (dataVideo) {
					if (hasValue(dataVideo)) {
						var video = JSON.parse(dataVideo);

						//var caminhoVideo = Endereco() + "/Informacao/getDownloadArquivo/?cd_video=" + cd_video;
						var caminhoVideo = Endereco() + "/Videos/" + video.retorno.no_arquivo_video;

						dom.byId("nomeVid").innerHTML = "Nome do Vídeo: " + video.retorno.no_video;
						reproduzirVideo(caminhoVideo);
					} else {
						showMessageErro("apresentadorMensagemVideoDetail", "Não foi possível reproduzir o vídeo, tente novamente ou entre em contato com suporte técnico.")
					}
					
					
				});
				
			} catch (e) {
				
				postGerarLog(e);
			}
			

		});
	});
	showCarregando();

}

function showMessageErro (el, mensagem) {
		var mensagensWeb = new Array();
		mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, mensagem);
		apresentaMensagem(el, mensagensWeb, true);
	
};