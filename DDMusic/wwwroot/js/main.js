function dowloadMusic(urlMusic) {
    var link = document.createElement('a');
    link.href = '../audio/' + urlMusic;
    link.download = urlMusic;
    link.dispatchEvent(new MouseEvent('click'));
}
function createPlayList(idSong) {
    $("#idSong").val(idSong);
    $("#modalAdd").modal('show');
}