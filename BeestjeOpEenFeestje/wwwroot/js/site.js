function selectImage(imagePath) {
                document.getElementById('ImageURL').value = imagePath;

                var button = document.querySelector('.dropbtn');
                button.innerHTML = 'Gekozen plaatje: <img src="' + imagePath + '" alt="Selected Image" style="width: 50px; height: auto;">';
}