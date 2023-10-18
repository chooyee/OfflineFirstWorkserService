class Numberbox extends Base
{    
	constructor(jsonConfig) {   
		super();     
		this.group = "Basic"
		this.name = "Numberbox";
		this.version = "v1";
		if (jsonConfig!== void 0)        
		  this.config = JSON.parse(jsonConfig);        
		else
		  this.config =  {};       
	 
		//console.log(this.config)
		let config = this.config;
		if (config.propertyName === void 0) { config.propertyName = ''; }
		if (config.type === void 0) { config.type = 'Numberbox'; }
		if (config.label === void 0) { config.label = 'My Number box'; }
		if (config.labelPosition === void 0) { config.labelPosition = 'Left-Left'; }
		//if (this.isPropExists(config, 'labelPosition')) { config.labelPosition = 'top'; }
		if (config.labelWidth === void 0) { config.labelWidth = 30; }
		if (config.labelMargin === void 0) { config.labelMargin = 3; }
		if (config.value === void 0) { config.value = ""; }
		if (config.pattern === void 0) { config.pattern = "/[^0-9]/g"; }
	   
		if (config.validation === void 0) {
		  config.validation = {}
		  config.validation.mandatory = false;

		  if (config.validation.lengthCheck === void 0) {
			config.validation.lengthCheck = {};
			config.validation.lengthCheck.min = 0;
			config.validation.lengthCheck.max = 0;
		  };
		};
		
	  
		if (config.formBuilderMode === void 0) { config.formBuilderMode = false; }
		//this.editFormJson = this.getEditFormJson();
	}

	getEditFormJson = () =>{
	  return {
		"label": "Number Field Component Edit Form",
		"tabs": [
		   {"tabLabel": "Display",
			 "tabId":"home",
			"components": [              
			  {
				"propertyName":"label",
				"label": "Label",
				"placeholder": "My Numberbox label",
				"description": "Enter the label for this number field", 
				"validation":{"mandatory": true, "lengthCheck": {"min": 0, "max":0}},
				"value":this.config.label,           
				"type": "Textbox"
			  },
			  {
				"propertyName":"labelPosition",
				"label": "Label Position",
				"placeholder": "Label Position",
				"description": "Set the position of the label", 
				"options":[
				  {"Top":"Top"},
				  {"Left-Left":"Left"},
				  {"None":"None"}
				],
				"value": this.config.labelPosition,           
				"type": "Select"
			  },
			  {
				"propertyName":"description",
				"label": "Descriptions (Optional)",
				"description": "Description for the number field", 
				"type": "Textbox",
				"value": this.config.description
			  },
			  {
				"propertyName":"placeholder",
				"label": "Placeholder (Optional)",
				"placeholder": "Placeholder on the number field",
				"type": "Textbox",
				"value": this.config.placeholder
			  },
			  {
				"propertyName":"prefix",
				"label": "Prefix (Optional)",
				"placeholder": "Prefix",
				"description": "Set the prefix infront of the text field, for ex: {RM} [numberbox]", 
				"type": "Textbox",
				"value": this.config.prefix
			  },
			  {
				"propertyName":"suffix",
				"label": "Suffix (Optional)",
				"placeholder": "Suffix",
				"description": "Set the prefix infront of the text field, for ex: [numberbox] {RM}", 
				"type": "Textbox",
				"value": this.config.suffix
			  },
			  {
				"propertyName":"customCss",
				"label": "Custom CSS",
				"placeholder": "custom css",
				"description": "Set the custom css, for ex: form-control form-item", 
				"type": "Textbox",
				"value": this.config.customCss
			  }
			]
		  },
		  {
			"tabLabel": "Validation",
			"tabId":"validate",
			"components": [
			  {
				"propertyGroup":"validation",
				"propertyType":"mandatory",
				"propertyName":"validation-mandatory",
				"label": "This is mandatory field?",
				"value":this.config.validation.mandatory, 
				"type": "Checkbox"
			  },
			  {
				"propertyGroup":"validation",
				"propertyType":"lengthCheck",
				"propertyName":"validation-lengthCheck-min",
				"label": "Minimum Length (Optional)",
				"placeholder": "Minimum Length",  
				"type": "Numberbox",
				"value": this.config.validation.lengthCheck.min
			  },
			  {
				"propertyGroup":"validation",
				"propertyType":"lengthCheck",
				"propertyName":"validation-lengthCheck-max",
				"label": "Maximum Length (Optional)",
				"placeholder": "Maximum Length",
				"type": "Numberbox",
				"value": this.config.validation.lengthCheck.max
			  }
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
			  },
			  {
				"propertyName":"value",
				"label": "Default Value",
				"value": this.config.value, 
				"type": "Numberbox"
			  },
			  {
				"propertyName":"pattern",
				"label": "Regex Pattern",
				"value": this.config.pattern, 
				"type": "Textbox"
			  }
			]
		  }
		]
	  };
	}

	renderElement = (config, elementId)=>{
	  //propertyName, elementId, prefix, suffix, placeholder, letterCase, required, val
	  const prefix = config.prefix;
	  const suffix = config.suffix;     
	  const desc = config.description;
	  //===================================================================================
		//render Input Group
		//===================================================================================
	   
		const divInputGroup = this.createElement("div", {"class":"input-group"});       

		if (!this.isNullOrEmpty(prefix)){
		  const divPrefix = this.createElement("div", {"ref":"prefix", "class":"input-group-text"})  //<div ref="prefix" class="input-group-text">prefix</div>
		  divPrefix.innerHTML = prefix;
		  divInputGroup.appendChild(divPrefix);
		}
		
		//<input aria-required="true" aria-labelledby="l-epx0fbk-textField d-epx0fbk-textField" id="epx0fbk-textField" value="" spellcheck="true" placeholder="Placeholder" autocomplete="off" lang="en" class="form-control" type="text" name="data[textField]" ref="input">
		const divFormFloat = this.createElement("div", {"class":"form-floating"}); 
		divInputGroup.appendChild(divFormFloat);

		const inputEl = this.renderRawElement(config, elementId);
		
		divFormFloat.appendChild(inputEl);
		const label = this.createElement("label",{"for":`${this.name}-${elementId}`});
		label.innerHTML = config.label;
		divFormFloat.appendChild(label);

		if (!this.isNullOrEmpty(suffix))
		{
		  const divSuffix = this.createElement("div", {"ref":"suffix", "class":"input-group-text"})  //<div ref="prefix" class="input-group-text">prefix</div>
		  divSuffix.innerHTML = suffix;
		  divInputGroup.appendChild(divSuffix);
		}

	   
		const divElement = this.createElement("div", {"ref":"element"});
		divElement.appendChild(divInputGroup);

		if (!this.isNullOrEmpty(desc))
		{
		  //<div class="form-text text-muted" id="d-egxfg08-checkbox">testew er ewrewr ewrewr ewre w</div>
		  const divDesc = this.createElement("div", {"class":"form-text text-muted"});
		  divDesc.innerHTML = desc;
		  divElement.appendChild(divDesc);
		}

		return divElement;
		//===================================================================================
		//end render Input Group
		//===================================================================================
	}

	renderRawElement = (config, elementId)=>{   
	  const propertyName = config.propertyName;
	  const placeholder = config.placeholder;
	  const val = config.value;

	  const inputEl =  this.createElement("input", {"placeholder":config.label,"id":`${this.name}-${elementId}`, "class":"form-control", "type":"text", "ref":"input"});
	  inputEl.setAttribute("data-pattern", config.pattern);
	  //Must implement!
	  if (!this.isNullOrEmpty(config.propertyId))
	  {
		inputEl.setAttribute("data-id", config.propertyId);
	  }

	  if (!this.isNullOrEmpty(propertyName))
	  {
		inputEl.setAttribute("data-property",propertyName);
	  }
	  
	  if (!this.isNullOrEmpty(val))
	  {
		inputEl.setAttribute("value",val);
	  }


	  if (!this.isNullOrEmpty(placeholder))
	  {
		inputEl.setAttribute("placeholder", placeholder);
	  }

	   
	  if (this.isPropExists(config,"validation"))
	  {
		
		if (this.isPropExists(config.validation, "mandatory"))
		{
		  inputEl.classList.add("mandatory");
		}
		if (this.isPropExists(config.validation, "lengthCheck"))
		{
		  
		  if (typeof Number(config.validation.lengthCheck.min) == 'number'){
			if (+config.validation.lengthCheck.min > 0)
			  inputEl.setAttribute("minLength", config.validation.lengthCheck.min);
		  }

		  if (typeof Number(config.validation.lengthCheck.max) == 'number'){
			
			if (+config.validation.lengthCheck.max > 0){
			  inputEl.setAttribute("maxLength", config.validation.lengthCheck.max);
			}
		  }
		}

	  }
	   
	  this.attachComponentEventListener(inputEl);
	  return inputEl;
	}

	attachComponentEventListener=(inputEl)=>{
		inputEl.removeEventListener("input",this.numberFuncHandler);
		inputEl.addEventListener("input",this.numberFuncHandler);
	}

	numberFuncHandler=(event)=>{
		event.preventDefault();
		const currentValue = event.target.value;

		// Remove all non-numeric characters from the value
		///[^0-9]/g
		const pattern = event.target.getAttribute("data-pattern");
		const numericValue = currentValue.replace(eval(pattern), '');
		//console.log(numericValue)
		// Update the input's value with the numeric value
		event.target.value = numericValue;
	}


}
