using System.Text;
using System.Security.Cryptography;

namespace WJMaxDecryptor
{
    public partial class Form1 : Form
    {
        private int totalFiles = 0;
        private int processedFiles = 0;

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

        private void CountTotalFiles(string path)
        {
            try
            {
                // 현재 디렉토리의 파일 수 계산
                totalFiles += Directory.GetFiles(path).Length;

                // 하위 디렉토리 재귀적 처리
                foreach (string dir in Directory.GetDirectories(path))
                {
                    string dirName = Path.GetFileName(dir);

                    // WJMAX_Data 폴더의 직계 자식 폴더 중 제외할 폴더는 건너뛰기
                    if (Path.GetFileName(path) == "WJMAX_Data" &&
                        ROOT_SKIP_FOLDERS.Contains(dirName))
                    {
                        continue;
                    }

                    CountTotalFiles(dir);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"파일 수 계산 중 오류 발생: {ex.Message}");
            }
        }

        private void UpdateProgress(string currentFile)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateProgress), currentFile);
                return;
            }

            processedFiles++;
            progressBar.Value = (int)((float)processedFiles / totalFiles * 100);
            statusLabel.Text = $"처리 중: {currentFile}\n진행률: {processedFiles}/{totalFiles} ({progressBar.Value}%)";
            Application.DoEvents();
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

        //private void encryptBtn_Click(object sender, EventArgs e)
        //{
        //    byte[] bytes = loadFileByModal();
        //    if (bytes == null)
        //    {
        //        MessageBox.Show("Invalid File!");
        //        return;
        //    }

        //    writeFileByModal(
        //        Encoding.UTF8.GetBytes(
        //            Convert.ToBase64String(
        //                GetRijndaelManaged().CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length)
        //            )
        //        )
        //    );
        //}

        //private void decryptBtn_Click(object sender, EventArgs e)
        //{
        //    byte[] bytes = loadFileByModal();
        //    if (bytes == null)
        //    {
        //        MessageBox.Show("Invalid File!");
        //        return;
        //    }

        //    String base64 = Encoding.UTF8.GetString(bytes);
        //    byte[] data = Convert.FromBase64String(base64);
        //    writeFileByModal(
        //        GetRijndaelManaged().CreateDecryptor().TransformFinalBlock(data, 0, data.Length)
        //    );
        //}

        private readonly string[] ENCRYPT_EXTENSIONS = { ".info", ".wak", ".txt" };
        private readonly string[] SKIP_EXTENSIONS = { ".assets", ".config", ".resS", ".dll", "resource" };
        private readonly HashSet<string> SKIP_FILENAMES = Enumerable.Range(1, 99)
            .Select(i => $"level{i}")
            .Concat(new[] {
                "unity default resources",
                "unity_builtin_extra",
                "RuntimeInitializeOnLoads",
                "ScriptingAssemblies",
                "globalgamemanagers",
            })
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        private readonly string[] ROOT_SKIP_FOLDERS = { "Managed", "Plugins", "Resources", "StreamingAssets" };

        private void ProcessDirectory(string sourcePath, string targetPath, bool isEncrypt, bool isRoot = true)
        {
            // WJMAX_Data 폴더의 직계 자식 폴더 중 제외할 폴더 체크
            if (isRoot &&
                Path.GetFileName(sourcePath) == "WJMAX_Data" &&
                ROOT_SKIP_FOLDERS.Contains(Path.GetFileName(sourcePath)))
            {
                return;
            }

            // 대상 디렉토리가 없으면 생성
            Directory.CreateDirectory(targetPath);

            // 모든 파일 처리
            foreach (string sourceFile in Directory.GetFiles(sourcePath))
            {
                string fileName = Path.GetFileName(sourceFile);
                string extension = Path.GetExtension(sourceFile).ToLower();
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(sourceFile);

                // 제외할 파일은 건너뛰기
                if (SKIP_EXTENSIONS.Contains(extension, StringComparer.OrdinalIgnoreCase) ||
                    SKIP_FILENAMES.Contains(fileNameWithoutExt))
                {
                    continue;
                }

                string targetFile = Path.Combine(targetPath, fileName);

                if (ENCRYPT_EXTENSIONS.Contains(extension, StringComparer.OrdinalIgnoreCase))
                {
                    ProcessEncryptionFile(sourceFile, targetFile, isEncrypt);
                }
                else
                {
                    File.Copy(sourceFile, targetFile, true);
                }

                UpdateProgress(fileName);  // 진행 상황 업데이트
            }

            // 하위 디렉토리 재귀적 처리
            foreach (string sourceDir in Directory.GetDirectories(sourcePath))
            {
                string dirName = Path.GetFileName(sourceDir);

                // WJMAX_Data 폴더의 직계 자식 폴더 중 제외할 폴더 체크
                if (Path.GetFileName(sourcePath) == "WJMAX_Data" &&
                    ROOT_SKIP_FOLDERS.Contains(dirName))
                {
                    continue;
                }

                string targetDir = Path.Combine(targetPath, dirName);
                // 하위 폴더 처리시에는 isRoot를 false로 전달
                ProcessDirectory(sourceDir, targetDir, isEncrypt, false);
            }
        }

        private void ProcessEncryptionFile(string sourceFile, string targetFile, bool isEncrypt)
        {
            byte[] sourceBytes = File.ReadAllBytes(sourceFile);
            byte[] resultBytes;

            if (isEncrypt)
            {
                resultBytes = Encoding.UTF8.GetBytes(
                    Convert.ToBase64String(
                        GetRijndaelManaged().CreateEncryptor().TransformFinalBlock(sourceBytes, 0, sourceBytes.Length)
                    )
                );
            }
            else
            {
                string base64 = Encoding.UTF8.GetString(sourceBytes);
                byte[] data = Convert.FromBase64String(base64);
                resultBytes = GetRijndaelManaged().CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
            }

            File.WriteAllBytes(targetFile, resultBytes);
        }

        private void encryptBtn_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog sourceDialog = new FolderBrowserDialog())
            {
                sourceDialog.Description = "소스 폴더를 선택하세요";
                if (sourceDialog.ShowDialog() != DialogResult.OK) return;

                using (FolderBrowserDialog targetDialog = new FolderBrowserDialog())
                {
                    targetDialog.Description = "결과물 폴더를 선택하세요";
                    if (targetDialog.ShowDialog() != DialogResult.OK) return;

                    try
                    {
                        // 진행 상황 초기화
                        totalFiles = 0;
                        processedFiles = 0;
                        progressBar.Value = 0;
                        statusLabel.Text = "파일 수 계산 중...";

                        // 전체 파일 수 계산
                        CountTotalFiles(sourceDialog.SelectedPath);

                        // 처리 시작
                        ProcessDirectory(sourceDialog.SelectedPath, targetDialog.SelectedPath, true);
                        MessageBox.Show("암호화가 완료되었습니다.");

                        // 완료 후 초기화
                        statusLabel.Text = "완료됨";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"오류가 발생했습니다: {ex.Message}");
                    }
                }
            }
        }

        private void decryptBtn_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog sourceDialog = new FolderBrowserDialog())
            {
                sourceDialog.Description = "소스 폴더를 선택하세요";
                if (sourceDialog.ShowDialog() != DialogResult.OK) return;

                using (FolderBrowserDialog targetDialog = new FolderBrowserDialog())
                {
                    targetDialog.Description = "결과물 폴더를 선택하세요";
                    if (targetDialog.ShowDialog() != DialogResult.OK) return;
                    try
                    {
                        // 진행 상황 초기화
                        totalFiles = 0;
                        processedFiles = 0;
                        progressBar.Value = 0;
                        statusLabel.Text = "파일 수 계산 중...";
                        // 전체 파일 수 계산
                        CountTotalFiles(sourceDialog.SelectedPath);
                        // 처리 시작
                        ProcessDirectory(sourceDialog.SelectedPath, targetDialog.SelectedPath, false);
                        MessageBox.Show("복호화가 완료되었습니다.");
                        // 완료 후 초기화
                        statusLabel.Text = "완료됨";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"오류가 발생했습니다: {ex.Message}");
                    }
                }
            }
        }
    }
}
