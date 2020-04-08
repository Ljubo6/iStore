 var roxyFileman = '/lib/fileman/index.html';
 $(function () {
     CKEDITOR.replace('Body', {
         filebrowserBrowseUrl: roxyFileman,
         filebrowserImageBrowseUrl: roxyFileman + '?type=image',
         removeDialogTabs: 'link:upload;image:upload'
     });
 });