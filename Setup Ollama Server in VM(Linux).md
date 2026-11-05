
**After creating the VM instance :-

# تشغيل Ollama على Oracle Cloud VM وربطه بجهاز Windows

## 1. الاتصال بالسيرفر من Windows

افتح PowerShell:

```
cd $HOME\Downloads
```

تعديل صلاحيات ملف SSH Key:

```
icacls "<FileName>.key" /inheritance:r
```

```
icacls "<FileName>.key" /grant:r "$($env:USERNAME):(R)"
```

الاتصال بالسيرفر:

```
ssh -i ".\<FileName>.key" opc@<PUBLIC_IP_ADDRESS>
```

استبدل `<PUBLIC_IP_ADDRESS>` بعنوان الـ Public IP الخاص بالسيرفر.

## 2. تثبيت Ollama على Oracle Linux

الأوامر التالية تُنفّذ داخل SSH على السيرفر.

تثبيت مكتبة `zstd`:

```
sudo dnf install zstd -y
```

تحميل وتثبيت Ollama:

```
curl -fsSL https://ollama.com/install.sh | sh
```

## 3. تعديل إعدادات Ollama

إنشاء مجلد إعدادات الخدمة:

```
sudo mkdir -p /etc/systemd/system/ollama.service.d
```

فتح ملف الإعدادات:

```
sudo nano /etc/systemd/system/ollama.service.d/override.conf
```

ضع داخله:

```
[Service]
Environment="OLLAMA_HOST=0.0.0.0:11434"
```

حفظ الملف والخروج من Nano:

1. `Ctrl + O`
    
2. `Enter`
    
3. `Ctrl + X`
    

إعادة تحميل إعدادات Systemd:

```
sudo systemctl daemon-reload
```

إعادة تشغيل Ollama:

```
sudo systemctl restart ollama
```

## 4. إعداد Firewall داخل السيرفر

اسمح بالوصول إلى Ollama API من عنوان IP جهازك فقط:

```
sudo firewall-cmd --permanent --add-rich-rule='rule family="ipv4" source address="<YOUR_PUBLIC_IP>/32" port port="11434" protocol="tcp" accept'
```

تطبيق القاعدة:

```
sudo firewall-cmd --reload
```

استبدل `<YOUR_PUBLIC_IP>` بعنوان الـ Public IP الخاص بجهازك.

## 5. اختبار Ollama API من Windows

افتح PowerShell على جهازك.

اختبار الاتصال وعرض الموديلات المثبتة:

```
curl.exe http://<PUBLIC_IP_ADDRESS>:11434/api/tags
```

## 6. إرسال Prompt إلى الموديل

الموديل المستخدم:

`qwen2.5-coder:7b`

Endpoint:

```
POST http://<PUBLIC_IP_ADDRESS>:11434/api/generate
```

Request Body:

```
{
  "model": "qwen2.5-coder:7b",
  "prompt": "Say hello",
  "stream": false
}
```

PowerShell Command:

```
$body = @{
    model = "qwen2.5-coder:7b"
    prompt = "Say hello"
    stream = $false
} | ConvertTo-Json

Invoke-RestMethod `
    -Uri "http://<PUBLIC_IP_ADDRESS>:11434/api/generate" `
    -Method POST `
    -ContentType "application/json" `
    -Body $body
```