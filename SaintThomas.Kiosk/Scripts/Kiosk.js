$(document).ready(function () {
    $(".fancybox").click(function () {
        $.fancybox.showLoading();

        var wrap = $('<div id="dummy"></div>').appendTo('body');
        var el = $(this).clone().appendTo(wrap);

        el.oembed(null, {
            embedMethod: 'replace',
            youtube: { auto: true },
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
            }
        });
        return false;
    });
});