class Radio extends Base
{    
	constructor(jsonConfig) {   
		super();     
		this.group = "Basic"
		this.name = "Radio";
		this.version = "v1";
		if (jsonConfig!== void 0)        
			this.config = JSON.parse(jsonConfig);        
		else
			this.config =  {};       
	 
		let config = this.config;
		if (config.propertyName === void 0) { config.propertyName = ''; }
		if (config.type === void 0) { config.type = 'Radio'; }
		if (config.label === void 0) { config.label = 'My Radio'; }
		if (config.labelPosition === void 0) { config.labelPosition = 'Left-Left'; }
		if (config.labelWidth === void 0) { config.labelWidth = 30; }
		if (config.labelMargin === void 0) { config.labelMargin = 3; }
		if (config.options === void 0) { config.options = ["option 1", "option 2", "option 3"]; }
		if (config.formBuilderMode === void 0) { config.formBuilderMode = false; }
		if (config.value === void 0) { config.value = ""; }
		if (config.validation === void 0) {
			config.validation = {}
			config.validation.mandatory = false;

		};
	    
		//this.editFormJson = this.getEditFormJson();
	}

	getEditFormJson = () =>{
		return {
		  "label": "Radio component",
		  "tabs": [
		  {
			  "tabLabel": "Display",
			  "tabId":"home",
			  "components": [
				{
				  "propertyName":"label",
				  "label": "Label",
				  "placeholder": "Label for this element",
				  "description": "Enter the label for this element",     
				  "validate": {"mandatory": true},          
				  "value": this.config.label, 
				  "type": "Textbox"
				},
				{
				  "propertyName":"labelPosition",
				  "label": "Label Position",
				  "placeholder": "Label Position",
				  "description": "Set the position of the label", 
				  "options":[
					{"Top":"Top"},
					{"Left-Left":"Left"}
				  ],
				  "value": this.config.labelPosition,           
				  "type": "Select"
				},
				{
				  "propertyName":"description",
				  "label": "Descriptions",
				  "description": "Description for the element", 
				  "type": "Textbox",
				  "value": this.config.description
				}
			  ]
			},
			{
			  "tabLabel": "Data",
			  "tabId":"data",
			  "components": [
				{                
				  "propertyId":"targetEditFormOptions",
				  "propertyName":"options",
				  "label": "Dropdown Options",
				  "description": "Enter the dropdown options",
				  "type": "Textarea",
				  "value":this.optionsToText(this.config.options)
				},
				{
				  "propertyId":"SourceEditFormOptions",                
				  "propertyName":"value",
				  "label": "Default Select",
				  "description": "Enter the dropdown default",
				  "type": "Select",
				  "options":"No default\n" + this.optionsToText(this.config.options),
				  "value":this.config.value,
				  "events":[
					  {
						  "type":"listenToTarget",                    
						  "target":"targetEditFormOptions",
						  "event":"input",
						  "handler":"editFormDefaultValueListener"
					  }
				  ]
				}
			  ]
			}
		  ]
		};
	}


	optionsToText = function(options)
	{
		if (Array.isArray(options))
			return options.join("\n");
		else 
		{
			return options;
		}
	}


	renderElement = (config, elementId)=>{
		//propertyName, elementId, placeholder, options, required
	   
		const desc = config.description;
		//===================================================================================
		//render Input Group
		//===================================================================================
	   
		const divInputGroup = this.createElement("div", {"class":"input-group"});       

		const inputEl = this.renderRawElement(config, elementId);
	   
		divInputGroup.appendChild(inputEl);
	  
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
		let options = config.options;
		const defaultValue = config.value;
		const inputEl = this.createElement("div", {"data-property":propertyName,"id":`${this.name}-${elementId}`, "class":"form-radio radio", "ref":"radioGroup"});
		
		if (!this.isNullOrEmpty(config.propertyId))
		{
		inputEl.setAttribute("data-id", config.propertyId);
		}

		if (this.trueTypeOf(options)=="string")
		{
			options = options.split("\n");
		}

		let radioRequired = "";
		if (this.isPropExists(config,"validation"))
		{		
			if (this.isPropExists(config.validation, "mandatory"))
			{
				//inputEl.classList.add("mandatory");
				radioRequired = "required";
			}
		}

		options.forEach(option => {
			if (this.trueTypeOf(option)=="string")
			{
				//inputEl.add(new Option(option, option));  
				inputEl.appendChild(this.renderRadioComponent(option, option, elementId, radioRequired));
			}
			else{
				const key = Object.keys(option)[0];
				//inputEl.add(new Option(option[key],key));  
				inputEl.appendChild(this.renderRadioComponent(option[key], key, elementId, radioRequired));
			}
		});

		
		if (this.isPropExists(config,"validation"))
		{		
			if (this.isPropExists(config.validation, "mandatory"))
			{
				inputEl.classList.add("mandatory");
			}
		}

		if (!this.isNullOrEmpty(defaultValue)) {this.setDefaultOption(inputEl, defaultValue)}
		return inputEl;
	}

	renderRadioComponent=(label, optionValue, elementId, radioRequired)=>{
		const divRadioWrapper = this.createElement("div", {"class":"radio form-check", "ref":"wrapper"});

		const radioOptionId = `Radio-${elementId}-${this.getShortUUID()}`;
		//<input role="radio" id="egz1rha-ehjhwoh--apple" value="apple" lang="en" class="form-check-input" type="radio" name="data[radio][egz1rha-ehjhwoh]" ref="input">
		const radio = this.createElement("input", {"id":radioOptionId,"name":`radio[${elementId}]`, "value":optionValue, "class":"form-check-input", "type": "radio","ref":"input"});
	
		if (!this.isNullOrEmpty(radioRequired))
		{
			radio.setAttribute("required", true);
		}
		//<label for="egz1rha-ehjhwoh--apple" class="form-check-label label-position-right"><span>apple</span></label>
		const labelElement = this.createElement("label",{"for":radioOptionId, "class":"form-check-label label-position-right"});
		const span = this.createElement("span",{});
		span.innerHTML = label;

		divRadioWrapper.appendChild(radio);
		divRadioWrapper.appendChild(labelElement);
		labelElement.appendChild(span);

		return divRadioWrapper;

	}

	setDefaultOption(selectElement, defaultValue) {

		const allRadios = selectElement.querySelectorAll("[ref='input']");
		allRadios.forEach((radio)=>{
			if (radio.value===defaultValue)
			{
				radio.checked=true;
				radio.setAttribute("checked", "checked");
			}
			else{
				radio.removeAttribute("checked");
			}
		})
	}

	//========================================================================
	//Custom Event Handler
	//=========================================================================
	


}
