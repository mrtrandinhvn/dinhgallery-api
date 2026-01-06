echo Make sure that docker is running first
docker build -t dinhgalleryapi:2.3.0 -f ./DinhGalleryApi/Dockerfile .
docker save dinhgalleryapi:2.3.0 > dinhgalleryapi_2.3.0.tar
pause