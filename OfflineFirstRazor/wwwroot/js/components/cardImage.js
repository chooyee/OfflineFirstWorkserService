class CardImage extends Base
{    
	constructor(jsonConfig) {   
		super();     
		this.group = "Basic"
		this.name = "CardImage";
		this.version = "v1";
		if (jsonConfig!== void 0)        
		  this.config = JSON.parse(jsonConfig);        
		else
		  this.config =  {};       
	 
		//console.log(this.config)
		let config = this.config;
		if (config.propertyName === void 0) { config.propertyName = ''; }
		if (config.type === void 0) { config.type = 'CardImage'; }
		if (config.position === void 0) { config.position = 'left'; }
		if (config.image === void 0) { config.image = ""; }
		if (config.letterCase === void 0) { config.letterCase = "none"; }
		if (config.bodyText === void 0) { config.bodyText = ""; }
		if (config.headerText === void 0) { config.headerText = ""; }
		if (config.imageWidth === void 0) { config.imageWidth = ""; }
		
		if (config.formBuilderMode === void 0) { config.formBuilderMode = false; }
		//this.editFormJson = this.getEditFormJson();
	}

	getEditFormJson = () =>{
	  return {
		"label": "Card Image Component Edit Form",
		"tabs": [
		   {"tabLabel": "Display",
			 "tabId":"home",
			"components": [   
				{
					"propertyId" : "cardImageUpload",
					"propertyName":"image",
					"label": "Image",
					"description": "Select Image",
					"image":this.config.image,
					"type": "ImageUpload"
				},           
				{
					"propertyName":"bodyText",
					"label": "Image Body Text (optional)",
					"placeholder": "Image body text",
					"description": "Enter the text for this image card (optional)",
					"value":this.config.bodyText,           
					"type": "Textarea"
				},
				{
					"propertyName":"position",
					"label": "Header Position",
					"value":this.config.position,       
					"description": "Select the position for the header", 
					"options":[
					{"left":"left"},
					{"center":"center"},
					{"right":"right"}
					],    
					"type": "Select"
				}, 
				{ 
					"propertyName":"imageWidth",
					"label": "Image Width",  
					"type": "Numberbox",
					"suffix":"rem",
					"value": this.config.imageWidth
				},
			]
		  },
		  {
			"tabLabel": "Data",
			"tabId":"data",
			"components": [
			  {
				"propertyName":"propertyName",
				"label": "Data Property",
				"value": this.config.propertyName, 
				"type": "Textbox"
			  }
			]
		  }
		]
	  };
	}

	renderElement = (config, elementId)=>{
	 
	  //===================================================================================
		//render Input Group
		//===================================================================================
	   
		const divInputGroup = this.createElement("div", {"class":"card d-flex"});       

		if (config.position ==='center')
		{
			divInputGroup.classList.add("justify-content-center", "align-items-center")
		}
		else if (config.position ==='right')
		{
			divInputGroup.classList.add("justify-content-end", "align-items-right")
		}
		const inputEl = this.renderRawElement(config, elementId);
		
		divInputGroup.appendChild(inputEl);

	 
		if (!this.isNullOrEmpty(config.bodyText))
		{
			//<div class="form-text text-muted" id="d-egxfg08-checkbox">testew er ewrewr ewrewr ewre w</div>
			const cardBody = this.createElement("div", {"class":"card-body"});
			const pbody = this.createElement("p", {"class":"card-text d-flex"});     
			if (config.position ==='center')
			{
				pbody.classList.add("justify-content-center", "align-items-center")
			}
			else if (config.position ==='right')
			{
				pbody.classList.add("justify-content-end", "align-items-right")
			}

			switch (config.letterCase)
			{
				case "upper":
					pbody.classList.add("text-uppercase");
				break;
				case "lower":
					pbody.classList.add("text-lowercase");
				break;
				case "capital":
					pbody.classList.add("text-capitalize");
				break;
			}

			pbody.innerHTML = config.bodyText;
			cardBody.appendChild(pbody);
			divInputGroup.appendChild(cardBody);
		}

		return divInputGroup;
		//===================================================================================
		//end render Input Group
		//===================================================================================
	}

	renderRawElement = (config, elementId)=>{   
		const propertyName = config.propertyName;

		const inputEl =  this.createElement("img", {"id":`${this.name}-${elementId}`, "class":"card-image-top img-fluid", "src":config.image, "ref":"input"});
		
		if (!this.isNullOrEmpty(config.imageWidth))
		{
			inputEl.style.width = config.imageWidth + "rem";
		}
		//Must implement!
		if (!this.isNullOrEmpty(config.propertyId))
		{
		inputEl.setAttribute("data-id", config.propertyId);
		}

		if (!this.isNullOrEmpty(propertyName))
		{
		inputEl.setAttribute("data-property",propertyName);
		}
		
		return inputEl;
	}

	executeButtonSave = async ()=>{
		//const imageUrl = "blob:http://localhost:8080/8db60f95-ed66-4a48-8820-0adc648d403c";

		const imageUpload = document.querySelector('[data-id="cardImageUpload"]');
		//console.log(imageUpload)
		const preview = document.getElementById(`${imageUpload.id}-preview`);
		//console.log(preview)
		const anchor = preview.querySelector("a");
		if (anchor)
		{
			const blobImageUrl = anchor.getAttribute("href");
			//console.log (imageUrl);
			this.config.blobImageUrl = blobImageUrl;

			const publicImgUrl =  await this.uploadImageToApi(blobImageUrl, "/api/v1/upload");
			this.config.image =  location.protocol + '//' + location.host + "/" + publicImgUrl;
			
			console.log (publicImgUrl);
			return publicImgUrl;
		}
		else{
			const img = preview.querySelector("img");
			this.config.image = img.src
			//console.log(this.config.image)
			return this.config.image;
		}

	}

	uploadImageToApi(imageUrl, apiEndpoint) {

		return fetch(imageUrl)
		.then((response) => response.blob())
		.then((blob) => {
			const formData = new FormData();
			formData.append("image", blob, blob.name);
			
			return fetch(apiEndpoint, {
				method: "POST",
				body: formData,
				})
				.then((response) => {
					if (!response.ok) {
						throw new Error(`HTTP error! Status: ${response.status}`);
					}
					//console.log("uploading image");
					return response.json(); // You can handle the API response here
				})
				.then((data) => {
					//console.log("Image uploaded successfully:", data);
					return data.img;
				})
				.catch((error) => {
					console.error("Error uploading image:", error);
				});
		});
	  }

}
