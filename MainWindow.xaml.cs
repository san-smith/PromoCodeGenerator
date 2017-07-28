using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace PromoCodeGenerator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();           
        }


        private void button1Click(object sender, RoutedEventArgs e)
        {
            int numCount;
            if (! int.TryParse(textBoxNumCount.Text, out numCount))
            {
                MessageBox.Show("Неверный ввод!");
                return;
            }

            int symCount;
            if (!int.TryParse(textBoxSymCount.Text, out symCount))
            {
                MessageBox.Show("Неверный ввод!");
                return;
            }

            int promoCount;
            if (!int.TryParse(textBoxPomoCount.Text, out promoCount))
            {
                MessageBox.Show("Неверный ввод!");
                return;
            }

            string firstSymbolsString = "";
            int firstNum = 0;
            int firstSym = 0;

            // Если указаны первые символы
            if ((bool)checkBox1.IsChecked)
            {
                // Считаем количество указанных цифр и букв
                foreach (char ch in textBoxPromoFirstSymbols.Text)
                {
                    if ("0123456789".IndexOf(ch) >= 0)
                        firstNum++;

                    if ("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(ch) >= 0)
                        firstSym++;
                }

                firstSymbolsString = textBoxPromoFirstSymbols.Text;


                if (firstNum > numCount)
                {
                    MessageBox.Show(String.Format("Общее количество цифр не может быть меньше\nколичества уже заданных в начале строки: {0} < {1}", numCount, firstNum));
                    return;
                }

                if (firstSym > symCount)
                {
                    MessageBox.Show(String.Format("Общее количество букв не может быть меньше\nколичества уже заданных в начале строки: {0} < {1}", symCount, firstSym));
                    return;
                }

                // Вычтем количество уже указанных цифр букв
                numCount -= firstNum;
                symCount -= firstSym;
            }

            int promoCodeLen = numCount + symCount;

            double maxPromoCount = 0;

            // Сколько всего можно создать промокодов?
            if ((bool)checkUpper.IsChecked)
            {
                maxPromoCount = Math.Pow(10, numCount) * Math.Pow(26, symCount); // Только заглавные
            }
            else
            {
                maxPromoCount = Math.Pow(10, numCount) * Math.Pow(52, symCount);
            }
                      

            if (promoCodeLen == 0)
            {
                MessageBox.Show("Невозможно сгенерировать больше одного промокода!\nКоличество указанных в начале строки символов равно общей длине кода");
                return;
            }

            if (maxPromoCount < promoCount)
            {
                MessageBox.Show(String.Format("Невозможно сгенерировать столько промокодов!\nМаксимально возможное число кодов меньше требуемого: {0} < {1}", maxPromoCount, promoCount));
                return;
            }

            string str = "";
            string num = "0123456789";
            string alph = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int alphLen = alph.Length;
            Random rnd = new Random();
            
            HashSet<string> promoSet = new HashSet<string>();


            while (promoSet.Count < promoCount)
            {
                str = firstSymbolsString;

                // Добавляем нужное количество цифр и букв ...
                StringBuilder ss = new StringBuilder();
                for (int i = 0; i < numCount; i++)
                {
                    ss.Append(num[rnd.Next(num.Length)]);
                }
                for (int i = 0; i < symCount; i++)
                {
                    ss.Append(alph[rnd.Next(alph.Length)]);
                }

                // ... и перемешиваем их
                for (int i = 0; i < ss.Length; i++)
                {
                    int j = rnd.Next(ss.Length);
                    var x = ss[j];
                    ss[j] = ss[i];
                    ss[i] = x;
                }

                str += ss.ToString();

                if ((bool)checkUpper.IsChecked)
                {
                    str = str.ToUpper();
                }

                promoSet.Add(str);
                
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*";
            if (sfd.ShowDialog() == true)
            {
                StreamWriter writer = new StreamWriter(sfd.FileName);
                foreach (string st in promoSet)
                {
                    writer.WriteLine(st);
                }
                writer.Close();
            }


        }

        private void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox ctrl = sender as TextBox;
            e.Handled = "0123456789".IndexOf(e.Text) < 0;//только цифры
        }

        private void textBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true; // если пробел, отклоняем ввод
            }

            if (e.Key == Key.Enter)
            {                
                FocusNavigationDirection focusDirection = FocusNavigationDirection.Next;

                TraversalRequest request = new TraversalRequest(focusDirection);
               
                UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;
               
                if (elementWithFocus != null)
                {
                    elementWithFocus.MoveFocus(request);
                }
            }
        }

        private void checkBox1_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)checkBox1.IsChecked)
            {
                textBoxPromoFirstSymbols.IsEnabled = true;
            }
            else
            {
                textBoxPromoFirstSymbols.IsEnabled = false;
                textBoxPromoFirstSymbols.Text = "";
            }

        }

        private void textBoxPromoFirstSymbols_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox ctrl = sender as TextBox;
            e.Handled = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(e.Text) < 0;//только цифры или англ. буквы
        }
    }
}
