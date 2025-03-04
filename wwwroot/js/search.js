// Trigger file input when "Upload Photo" button is clicked
document.getElementById('upload-photo').addEventListener('click', function () {
    document.getElementById('imageFile').click();
});

// Display image preview when a file is selected
document.getElementById('imageFile').addEventListener('change', function (event) {
    const file = event.target.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function (e) {
            const imagePreview = document.getElementById('photo-preview');
            imagePreview.src = e.target.result;
            imagePreview.style.display = 'block';
        };
        reader.readAsDataURL(file);
    }
});