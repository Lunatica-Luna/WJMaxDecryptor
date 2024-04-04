using System.Text;
using System.Security.Cryptography;

namespace WJMaxDecryptor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private RijndaelManaged GetRijndaelManaged()
        {
            byte[] bytes = Encoding.UTF8.GetBytes(keyInput.Text);
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            byte[] numArray = new byte[16];
            byte[] destinationArray = numArray;
            Array.Copy((Array)bytes, 0, (Array)destinationArray, 0, 16);
            rijndaelManaged.Key = numArray;
            rijndaelManaged.Mode = CipherMode.ECB;
            rijndaelManaged.Padding = PaddingMode.PKCS7;
            return rijndaelManaged;
        }

        private byte[] loadFileByModal()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                byte[] file = File.ReadAllBytes(dialog.FileName);
                return file;
            }

            return null;
        }

        private void writeFileByModal(byte[] bytes)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                File.WriteAllBytes(dialog.FileName, bytes);
            }
        }

        private void encryptBtn_Click(object sender, EventArgs e)
        {
            byte[] bytes = loadFileByModal();
            if (bytes == null)
            {
                MessageBox.Show("Invalid File!");
                return;
            }

            writeFileByModal(
                Encoding.UTF8.GetBytes(
                    Convert.ToBase64String(
                        GetRijndaelManaged().CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length)
                    )
                )
            );
        }

        private void decryptBtn_Click(object sender, EventArgs e)
        {
            byte[] bytes = loadFileByModal();
            if (bytes == null)
            {
                MessageBox.Show("Invalid File!");
                return;
            }

            String base64 = Encoding.UTF8.GetString(bytes);
            byte[] data = Convert.FromBase64String(base64);
            writeFileByModal(
                GetRijndaelManaged().CreateDecryptor().TransformFinalBlock(data, 0, data.Length)
            );
        }
    }
}
