
namespace mongoDBLearn
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private bool _action = true;
        private Mutex mutex = new Mutex();
        private void button1_Click(object sender, EventArgs e)
        {
            /*
            listBox1.Items.Clear();
            mainmethod(new string[] { "Hello" });
            listBox1.Items.Insert(0, "Action normale start à " + DateTime.Now.ToString());
            Task.Delay(1000);
            listBox1.Items.Insert(0, "Action normale fin à " + DateTime.Now.ToString());
            */
            listBox1.Items.Clear();
            listBox1.Items.Insert(0, "Action normale start à " + DateTime.Now.ToString());
           
                mongoOpsAsync();
           
           
        }
        private async Task doProcess()
        {
            var connectionstring = "mongodb://localhost:27017";
            var client = new MongoClient(connectionstring);
            var db = client.GetDatabase("inscriptions");
            var col = db.GetCollection<BsonDocument>("personnes");
            BsonDocument monDoc = null;
            
                monDoc = new BsonDocument
            {
                {"nom","MoiMême" },
                {"prenom","Prenom" },
                {"age",0 }
            };
                monDoc.Add("profession", "none");
                monDoc["adresse"] = "Paris";
               await col.InsertOneAsync(monDoc);

            //Cursor Async
            /*
                using (var cursor = await col.Find(new BsonDocument()).ToCursorAsync())
                {
                    while (await cursor.MoveNextAsync())
                    {
                        foreach (var doc in cursor.Current)
                        {
                            Console.WriteLine(doc);
                        }
                    }
                }
            */

            //List Async
            /*
                var list = await col.Find(new BsonDocument()).ToListAsync();
                foreach (var doc in list)
                {
                    Console.WriteLine(doc);
                }
            */
            //foreachAsync
            await col.Find(new BsonDocument())
                .ForEachAsync(doc => Console.WriteLine(doc));

          listBox1.Items.Insert(0, "Fin action " + DateTime.Now.ToString());
        }
        private async Task mongoOpsAsync()
        {
            //var settings = new MongoClientSettings
            //{


            //};
            //var connectionstring = "mongodb://localhost:27017,localhost:27018/?replicatSet=funny";
            await doProcess();
            //listBox1.Items.Insert(0, "Action Fin insert à " + DateTime.Now.ToString());
        }

            private async Task mainmethod(string[] args)
        {
            listBox1.Items.Insert(0, "Début Action Task à " + DateTime.Now.ToString());
            while (true)
            {
               
               
                if (_action) { 
                await Task.Delay(500);
                listBox1.Items.Insert(0, "Action Task à " + DateTime.Now.ToString());
                }
                else { 
                    listBox1.Items.Insert(0, "Action Task Stopped à " + DateTime.Now.ToString());
                await Task.Delay(500);
                }

            }
            listBox1.Items.Insert(0, "Fin Action Task à " + DateTime.Now.ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mutex.WaitOne();
            _action = true;
            mutex.ReleaseMutex();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            mutex.WaitOne();
            _action = false;
            mutex.ReleaseMutex();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox1.Items.Insert(0, "Fin action " + DateTime.Now.ToString());
            //doSearch();
            //doSearchSkipLimit();
            doHomeWork();
        }
        private async Task doSearch()
        {
            var connectionstring = "mongodb://localhost:27017";
            var client = new MongoClient(connectionstring);
            var db = client.GetDatabase("inscriptions");
            var col = db.GetCollection<BsonDocument>("personnes");
            BsonDocument monDoc = null;

            monDoc = new BsonDocument
            {
                {"prenom",textBox1.Text } 
            };
            var list1 = await col.Find(new BsonDocument(monDoc)).ToListAsync();
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.And(builder.Gt("age", 35), builder.Eq("prenom", textBox1.Text));
            var filter2 = builder.Gt("age", 35) & builder.Eq("prenom", textBox1.Text) ;
            var list = await col.Find(filter2).ToListAsync();
            foreach (var doc in list)
            {
                Console.WriteLine(doc);
            }
            listBox1.Items.Insert(0, "Fin action " + DateTime.Now.ToString());
            

        }

        private async Task doSearchSkipLimit()
        {
            var connectionstring = "mongodb://localhost:27017";
            var client = new MongoClient(connectionstring);
            var db = client.GetDatabase("inscriptions");
            var col = db.GetCollection<BsonDocument>("personnes");
            BsonDocument monDoc = null;

            monDoc = new BsonDocument
            {
                {"prenom",textBox1.Text }
            };
            var list = await col.Find(new BsonDocument())
                .Limit(4)
                .Skip(0)
                //.Sort("{age:1}")
                //.Sort(new BsonDocument("age",1))
                .Sort(Builders<BsonDocument>.Sort.Ascending("age"))
                //.Project("{nom:1,prenom:1,age:1,_id:0 }")
                //.Project(new BsonDocument("nom", 1).Add("age",1).Add("_id", 0))
                .Project(Builders<BsonDocument>.Projection.Include("nom").Include("age").Exclude("_id"))
                 .Limit(4)
                .ToListAsync();
            label1.Text = list.Count().ToString();
            foreach (var doc in list)
            {
                Console.WriteLine(doc);
            }
            listBox1.Items.Insert(0, "Fin action " + DateTime.Now.ToString());


        }

        private async Task doHomeWork()
        {

            var connectionstring = "mongodb://localhost:27017";
            var client = new MongoClient(connectionstring);
            //var db = client.GetDatabase("students");
            //var col = db.GetCollection<BsonDocument>("grades");
            //BsonDocument monDoc = null;
            //var builder = Builders<BsonDocument>.Filter;
            //var filter = builder.And(builder.Eq("type", "homework"));
            //var list = await col.Find(filter).ToListAsync();
            //foreach (var doc in list)
            //{
            //    col.DeleteOne(doc);
                
            //}


            var db = client.GetDatabase("students");
            var grades = db.GetCollection<BsonDocument>("grades");
            var list = await grades.Find(Builders<BsonDocument>.Filter.Eq("type", "homework"))
                .Sort(Builders<BsonDocument>.Sort.Ascending("student_id"))
                .Sort(Builders<BsonDocument>.Sort.Ascending("score"))
                .ToListAsync();


            Dictionary<int, BsonValue> dic = new Dictionary<int, BsonValue>();

            foreach (var doc in list)
            {
                if (!dic.ContainsKey(int.Parse(doc["student_id"].ToString())))
                {
                    dic.Add(int.Parse(doc["student_id"].ToString()), doc["score"]);
                    Console.WriteLine(doc["student_id"] + " " + doc["score"]);
                }
            }

            // Delete the lowest homework grade of each student
            foreach (int key in dic.Keys)
            {
                var builder = Builders<BsonDocument>.Filter;
                await grades.DeleteOneAsync(builder.And(builder.Eq("student_id", key), builder.Eq("score", dic[key])));
            }




            listBox1.Items.Insert(0, "Fin action " + DateTime.Now.ToString());
        }

            private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Go();
        }

        private async Task Go()
        {

            var connectionstring = "mongodb://localhost:27017";
            var client = new MongoClient(connectionstring);
            var db = client.GetDatabase("school");
            var students = db.GetCollection<BsonDocument>("students");
            var list = await students.Find(Builders<BsonDocument>.Filter.Eq("scores.type", "homework"))
                .Sort(Builders<BsonDocument>.Sort.Ascending("scores.score"))
                .ToListAsync();

            dynamic _data = new ExpandoObject();
            List<dynamic> lst = new List<dynamic>();

            foreach (var item in list)
            {
                 
               var bobo=  item["scores"] ;
                var arrayjson = bobo.AsBsonArray;
                var _index = 0;
                lst.Clear();
                foreach (var bsoOne in arrayjson)
                {

                    if (bsoOne["type"] == "homework")
                    {
                        
                        _data = new ExpandoObject();
                        _data.index = _index;
                        _data.value = Convert.ToDouble(bsoOne["score"]);
                        lst.Add(_data);
                        if (arrayjson.Count() == (_index+1))
                        {
                        

                            var min = lst.Count >0 ? lst[0].value: double.MinValue;
                            dynamic objmin = lst.Count > 0 ? lst[0] : null ;

                            for (int i = 0; i < lst.Count ; i++)
                            {
                                if( lst[i].value < min  )
                                {
                                min = lst[i].value;
                                objmin = lst[i];
                                }
                            }
                            var filter = Builders<BsonDocument>.Filter.Eq("_id", item["_id"]);
                            var update = Builders<BsonDocument>.Update.Set("scores." + objmin.index, "");
                            await students.UpdateOneAsync(filter, update);
                        }
                    }
                    _index++;


                }

             

            }

        }
    }
}
