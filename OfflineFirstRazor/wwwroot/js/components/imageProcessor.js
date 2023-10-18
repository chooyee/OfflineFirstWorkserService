function ImageProcessor(imagePreview, fileSelector) {
	this.refs = {};
	// this.refs.imagePreviews = document.querySelector('#previews');
	// this.refs.fileSelector = document.querySelector('input[type=file]');
	this.refs.imagePreviews =imagePreview;
	this.refs.fileSelector = fileSelector;
}
  
ImageProcessor.prototype.addImageBox = function (container) {
	container.innerHTML = "";
	let imageBox = document.createElement("div");
	let progressBox = document.createElement("progress");
	imageBox.appendChild(progressBox);
	container.appendChild(imageBox);
	return imageBox;
};
  
ImageProcessor.prototype.processFile = function (file) {
	if (!file) {
	  	return;
	}
	//console.log(file);
  
	let imageBox = this.addImageBox(this.refs.imagePreviews);
  
	// Load the data into an image
	new Promise(function (resolve, reject) {
		let rawImage = new Image();

		rawImage.addEventListener("load", function () {
			resolve(rawImage);
		});

		rawImage.src = URL.createObjectURL(file);
	}).
	then(function (rawImage) {
	  // Convert image to webp ObjectURL via a canvas blob
		return new Promise(function (resolve, reject) {
			let canvas = document.createElement('canvas');
			let ctx = canvas.getContext("2d");

			canvas.width = rawImage.width;
			canvas.height = rawImage.height;
			ctx.drawImage(rawImage, 0, 0);

			canvas.toBlob(function (blob) {
				resolve(URL.createObjectURL(blob));
			}, "image/webp");
		});
	}).
	then(function (imageURL) {
		// Load image for display on the page
		return new Promise(function (resolve, reject) {
			let scaledImg = new Image();

			scaledImg.addEventListener("load", function () {
				resolve({ imageURL, scaledImg });
			});

			scaledImg.setAttribute("src", imageURL);
			scaledImg.classList.add("img-thumbnail");
		});
	}).
	then(function (data) {
		// Inject into the DOM
		let imageLink = document.createElement("a");

		imageLink.setAttribute("href", data.imageURL);
		imageLink.setAttribute('download', `${file.name}.webp`);
		imageLink.appendChild(data.scaledImg);

		imageBox.innerHTML = "";
		imageBox.appendChild(imageLink);
	});
  };
  
ImageProcessor.prototype.processFiles = function (files) {
	for (let file of files) {
		this.processFile(file);
	}
};
  
ImageProcessor.prototype.fileSelectorChanged = function () {
	this.processFiles(this.refs.fileSelector.files);
	this.refs.fileSelector.value = "";
};
  
ImageProcessor.prototype.setDragDrop = function (area) {
	area.addEventListener("dragenter", this.dragenter, false);
	area.addEventListener("dragover", this.dragover, false);
	area.addEventListener("drop", (e) => this.drop(e), false);
};
  
ImageProcessor.prototype.dragenter = function (e) {
	e.stopPropagation();
	e.preventDefault();
};
  
ImageProcessor.prototype.dragover = function (e) {
	e.stopPropagation();
	e.preventDefault();
};
  
ImageProcessor.prototype.drop = function (e) {
	e.stopPropagation();
	e.preventDefault();
	this.processFiles(e.dataTransfer.files);
};
  
// const imageProcessor = new ImageProcessor();
// imageProcessor.setDragDrop(document.documentElement);
// imageProcessor.refs.fileSelector.addEventListener("change", () => imageProcessor.fileSelectorChanged());