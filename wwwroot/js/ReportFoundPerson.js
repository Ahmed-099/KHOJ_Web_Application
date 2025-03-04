const uploadPhotoBtn = document.getElementById('upload-photo');
const photoInput = document.querySelector('input[type="file"]');
const photoPreview = document.getElementById('photo-preview');

uploadPhotoBtn.addEventListener('click', () => {
    photoInput.click();
});

photoInput.addEventListener('change', (e) => {
    const file = e.target.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = (event) => {
            photoPreview.src = event.target.result;
            photoPreview.style.display = 'block';
        };
        reader.readAsDataURL(file);
    }
});

