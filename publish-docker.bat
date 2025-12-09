echo Make sure that docker is running first
docker build -t dinhgalleryapi:2.0.0 -f ./DinhGalleryApi/Dockerfile .
docker save dinhgalleryapi:2.0.0 > dinhgalleryapi_2.0.0.tar
pause