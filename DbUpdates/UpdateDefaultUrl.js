db.getCollection('MetaCards').update(
{ImageUrl: "https://www.pcgamesn.com/wp-content/uploads/2019/06/mtg-arena-core-set-2020.jpg"}, 
[    {
         $set: {ImageUrl: "/img/mtg-card-back.jpg"} 
     }
],
{ multi: true});
db.getCollection('Cards').update(
{ImageUrl: "https://www.pcgamesn.com/wp-content/uploads/2019/06/mtg-arena-core-set-2020.jpg"}, 
[    {
         $set: {ImageUrl: "/img/mtg-card-back.jpg"} 
     }
],
{ multi: true});