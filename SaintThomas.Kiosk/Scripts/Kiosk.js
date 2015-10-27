$(document).ready(function () {
    $(".fancybox").click(function () {
        $.fancybox.showLoading();

        var wrap = $('<div id="dummy"></div>').appendTo('body');
        var el = $(this).clone().appendTo(wrap);

        el.oembed(null, {
            embedMethod: 'replace',
            autoplay: true,
            afterEmbed: function (rez) {
                var what = $(rez.code);
                var type = 'html';
                var scrolling = 'no';

                if (rez.type == 'photo') {
                    what = what.find('img:first').attr('src');
                    type = 'image';
                } else if (rez.type === 'rich') {
                    scrolling = 'auto';
                }

                $.fancybox.open({
                    href: what,
                    type: type,
                    scrolling: scrolling,
                    title: rez.title || $(this).attr('title'),
                    width: 1024,
                    height: 768,
                    autoSize: false
                });

                wrap.remove();
            },
            onError: function () {
                $.fancybox.open(this);
            },
            youtube: { autoplay: true }
        });
        return false;
    });
});

// Load the IFrame Player API code asynchronously.
var tag = document.createElement('script');
tag.src = "https://www.youtube.com/player_api";
var firstScriptTag = document.getElementsByTagName('script')[0];
firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);
// Replace the 'ytplayer' element with an <iframe> and
// YouTube player after the API code downloads.
var player;
function onYouTubePlayerAPIReady() {
    player = new YT.Player('ytplayer', {
        height: '390',
        width: '640',
        videoId: 'M7lc1UVf-VE'
    });
}

function DoFullScreen() {

    var isInFullScreen = (document.fullScreenElement && document.fullScreenElement !== null) || // alternative standard method  
    (document.mozFullScreen || document.webkitIsFullScreen);

    var docElm = document.documentElement;
    if (!isInFullScreen) {

        if (docElm.requestFullscreen) {
            docElm.requestFullscreen();
        } else if (docElm.mozRequestFullScreen) {
            docElm.mozRequestFullScreen();
            alert("Mozilla entering fullscreen!");
        } else if (docElm.webkitRequestFullScreen) {
            docElm.webkitRequestFullScreen();
            alert("Webkit entering fullscreen!");
        }
    } else {
        if (document.exitFullscreen) {
            document.exitFullscreen();
        } else if (document.mozCancelFullScreen) {
            document.mozCancelFullScreen();
        } else if (document.webkitExitFullscreen) {
            document.webkitExitFullscreen();
        }
    }
}