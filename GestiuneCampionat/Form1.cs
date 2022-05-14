using GestiuneCampionat.DateCampionatDataSetTableAdapters;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestiuneCampionat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //afișăm datele din bază în tabelele din fereastră
            echipeTableAdapter.Fill(dateCampionatDataSet.Echipe);
            clasamentTableAdapter.Fill(dateCampionatDataSet.Clasament);
            meciuriEchipeTableAdapter.Fill(dateCampionatDataSet.MeciuriEchipe);
            echipeTableAdapter.Fill(dateCampionatDataSet.Echipe);
        }

        private void StergereEchipa_Click(object sender, EventArgs e)
        {
            if (echipeDataGridView.SelectedRows.Count > 0)
            {
                var confirmResult = MessageBox.Show("Doriți să ștergeți echipa " + echipeDataGridView.SelectedRows[0].Cells[2].FormattedValue.ToString() + " ?",
                                     "Confirmați ștergerea",
                                     MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    //citim id-ul echipei
                    int id = int.Parse(echipeDataGridView.SelectedRows[0].Cells["id"].FormattedValue.ToString());
                    
                    //ștergem echipa și meciurile ei
                    MeciuriTableAdapter meciuriTableAdapter = new MeciuriTableAdapter();
                    meciuriTableAdapter.DeleteMeciuriEchipaGazde(id);
                    meciuriTableAdapter.DeleteMeciuriEchipaOaspete(id);
                    echipeTableAdapter.DeleteEchipa(id);

                    //reafișăm tabele
                    echipeTableAdapter.Fill(dateCampionatDataSet.Echipe);
                    meciuriEchipeTableAdapter.Fill(dateCampionatDataSet.MeciuriEchipe);
                    clasamentTableAdapter.Fill(dateCampionatDataSet.Clasament);
                }
            }
        }

        private void ResetareCampionat_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Doriți să ștergeți toate meciurile?",
                                     "Confirmați ștergerea",
                                     MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                //ștergem toate meciurile
                MeciuriTableAdapter meciuriTableAdapter = new MeciuriTableAdapter();
                meciuriTableAdapter.DeleteToateMeciurile();
                dateCampionatDataSet.Meciuri.AcceptChanges();

                //reafișăm tabelele
                meciuriEchipeTableAdapter.Fill(dateCampionatDataSet.MeciuriEchipe);
                clasamentTableAdapter.Fill(dateCampionatDataSet.Clasament);
            }
        }

        private void AnulareMeci_Click(object sender, EventArgs e)
        {
            if (meciuriEchipeDataGridView.SelectedRows.Count > 0)
            {
                var confirmResult = MessageBox.Show("Doriți să ștergeți meciul " + 
                    meciuriEchipeDataGridView.SelectedRows[0].Cells[2].FormattedValue.ToString() + " - " +
                    meciuriEchipeDataGridView.SelectedRows[0].Cells[5].FormattedValue.ToString() + " ?",
                                     "Confirmați ștergerea",
                                     MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    //citim id-ul meciului
                    int id = int.Parse(meciuriEchipeDataGridView.SelectedRows[0].Cells[7].FormattedValue.ToString());
                    
                    //ștergem meciul din baza de date
                    MeciuriTableAdapter meciuriTableAdapter = new MeciuriTableAdapter();
                    meciuriTableAdapter.DeleteMeci(id);
                    dateCampionatDataSet.Meciuri.AcceptChanges();

                    //actualizare tabele
                    meciuriEchipeTableAdapter.Fill(dateCampionatDataSet.MeciuriEchipe);
                    clasamentTableAdapter.Fill(dateCampionatDataSet.Clasament);
                }
            }
        }

        private void buttonAdaugareMeci_Click(object sender, EventArgs e)
        {
            //citim id-urile echipelor
            int idGazde = int.Parse(comboGazde.SelectedValue.ToString());
            int idOaspeti = int.Parse(comboOaspeti.SelectedValue.ToString()); ;
            
            //verificăm datele
            if (idGazde == idOaspeti) //gazdele și oaspeții sunt aceeași echipă
                MessageBox.Show("Gazdele și oaspeții trebuie să fie echipe diferite.",
                    "Atenție!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (textEtapa.TextLength == 0) //nu e precizată etapa
                MessageBox.Show("Pentru a adăuga un meci trebuie să precizați etapa.",
                        "Atenție!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                try
                {
                    //citim datele din formular
                    string numeGazde = comboGazde.SelectedText;
                    string numeOaspeti = comboOaspeti.SelectedText;
                    int goluriGazde = int.Parse(textGoluriGazde.Text);
                    int goluriOaspeti = int.Parse(textGoluriOaspeti.Text);
                    int etapa = int.Parse(textEtapa.Text);

                    //trimitem datele către baza de date
                    MeciuriTableAdapter meciuriTableAdapter = new MeciuriTableAdapter();
                    meciuriTableAdapter.InsertMeci(idGazde, idOaspeti, goluriGazde, goluriOaspeti, etapa);
                    dateCampionatDataSet.AcceptChanges();

                    //actualizăm tabelele
                    meciuriEchipeTableAdapter.Fill(dateCampionatDataSet.MeciuriEchipe);
                    clasamentTableAdapter.Fill(dateCampionatDataSet.Clasament);

                    //resetăm golurire
                    textGoluriGazde.Text = "0";
                    textGoluriOaspeti.Text = "0";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Adăugarea meciului a eșuat. \n" +
                        "Asigurați-vă că ați introdus corect toate datele necesare. \n" +
                        "Mesaj eroare : " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }

        private void buttonAlegeImgInserare_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "png files (*.png)|*.png|jpg files (*.jpg)|*.jpg";
            fileDialog.RestoreDirectory = true;
            if( fileDialog.ShowDialog() == DialogResult.OK )
            {
                string filePath = fileDialog.FileName;
                Image img = Image.FromFile(filePath);
                pictureBoxInserare.Image = img;
            }
        }

        private void buttonAlegeImgActualizare_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "png files (*.png)|*.png|jpg files (*.jpg)|*.jpg";
            fileDialog.RestoreDirectory = true;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = fileDialog.FileName;
                Image img = Image.FromFile(filePath);
                pictureBoxActualizare.Image = img;
            }
        }

        private void buttonInserare_Click(object sender, EventArgs e)
        {
            //verificare existență nume
            if (textEchipaInserare.TextLength == 0)
                MessageBox.Show("Ați uitat să precizați numele echipei!", "Atenție",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            //verificare existență siglă
            if (pictureBoxInserare.Image == null)
                MessageBox.Show("Ați uitat să selectați o siglă!", "Atenție",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //dacă avem datele necesare
            else
            {
                //obținem datele echipei din fereastră
                ImageConverter imgConv = new ImageConverter();
                byte[] sigla = (byte[])imgConv.ConvertTo(pictureBoxInserare.Image, typeof(byte[]));
                string nume = textEchipaInserare.Text;

                //inserăm datele în baza de date
                try
                {
                    echipeTableAdapter.Insert(nume, sigla);
                    dateCampionatDataSet.AcceptChanges();
                } 
                catch (Exception ex)
                {
                    MessageBox.Show("Verificați datele introduse!\nExistă deja echipa?" +
                        "\nMesaj eroare: " + ex.Message, "Eroare",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                //actualizăm tabelele
                echipeTableAdapter.Fill(dateCampionatDataSet.Echipe);
                meciuriEchipeTableAdapter.Fill(dateCampionatDataSet.MeciuriEchipe);
                clasamentTableAdapter.Fill(dateCampionatDataSet.Clasament);
                
                //ștergem câmpurile din fereastră
                pictureBoxInserare.Image = null;
                textEchipaInserare.Text = null;
            }
        }

        private void buttonActualizare_Click(object sender, EventArgs e)
        {
            //verificare existență nume
            if (textEchipaActualizare.TextLength == 0)
                MessageBox.Show("Nu puteți salva o echipă fără nume!", "Atenție",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            //verificare existență siglă
            if (pictureBoxActualizare.Image == null)
                MessageBox.Show("Nu puteți salva o echipă fără siglă!", "Atenție",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //dacă avem datele necesare
            else
            {
                //obținem sigla curentă, numele și id-ul echipei
                ImageConverter imgConv = new ImageConverter();
                byte[] sigla = (byte[])imgConv.ConvertTo(pictureBoxActualizare.Image, typeof(byte[]));
                string nume = textEchipaActualizare.Text;
                int id = int.Parse(echipeDataGridView.SelectedRows[0].Cells[1].Value.ToString());

                //trimitem schimbările către baza de date
                try
                {
                    echipeTableAdapter.UpdateEchipa(sigla, nume, id);
                    dateCampionatDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Verificați datele introduse!\nExistă deja echipa?" +
                        "\nMesaj eroare: " + ex.Message, "Eroare",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                //actualizăm tabelele 
                echipeTableAdapter.Fill(dateCampionatDataSet.Echipe);
                meciuriEchipeTableAdapter.Fill(dateCampionatDataSet.MeciuriEchipe);
                clasamentTableAdapter.Fill(dateCampionatDataSet.Clasament);
            }
        }
    }
}
