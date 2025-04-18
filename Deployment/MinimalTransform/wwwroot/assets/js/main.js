// 1. First, extend the sampleData object to include flat (and tabular) examples for each format
const sampleData = {
    // Structured examples (original nested data)
    structuredJson: `{
  "company": {
    "name": "Acme Corp",
    "employees": [
      {"id": 1, "name": "Alice", "role": "Engineer", "skills": ["C#", "JS"]},
      {"id": 2, "name": "Bob", "role": "Manager", "reports": [
        {"id": 3, "name": "Charlie"},
        {"id": 4, "name": "Dana"}
      ]}
    ],
    "isPublic": true,
    "metadata": null
  }
}`,
    structuredXml: `<?xml version="1.0" encoding="UTF-8"?>
<company>
  <name>Acme Corp</name>
  <employees>
    <employee>
      <id>1</id>
      <name>Alice</name>
      <role>Engineer</role>
      <skills>
        <skill>C#</skill>
        <skill>JS</skill>
      </skills>
    </employee>
    <employee>
      <id>2</id>
      <name>Bob</name>
      <role>Manager</role>
      <reports>
        <report>
          <id>3</id>
          <name>Charlie</name>
        </report>
        <report>
          <id>4</id>
          <name>Dana</name>
        </report>
      </reports>
    </employee>
  </employees>
  <isPublic>true</isPublic>
</company>`,
    structuredYaml: `company:
  name: Acme Corp
  employees:
    - id: 1
      name: Alice
      role: Engineer
      skills: 
        - C#
        - JS
    - id: 2
      name: Bob
      role: Manager
      reports:
        - id: 3
          name: Charlie
        - id: 4
          name: Dana
  isPublic: true
  metadata: null`,
    structuredCsv: `id,name,role,skills
1,Alice,Engineer,"C#,JS"
2,Bob,Manager,`,

    // Flat examples (no nesting, simple key-value pairs)
    flatJson: `{
  "id": 1001,
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "age": 35,
  "active": true,
  "joinDate": "2022-03-15",
  "department": "Engineering",
  "salary": 85000
}`,
    flatXml: `<?xml version="1.0" encoding="UTF-8"?>
<employee>
  <id>1001</id>
  <firstName>John</firstName>
  <lastName>Doe</lastName>
  <email>john.doe@example.com</email>
  <age>35</age>
  <active>true</active>
  <joinDate>2022-03-15</joinDate>
  <department>Engineering</department>
  <salary>85000</salary>
</employee>`,
    flatYaml: `id: 1001
firstName: John
lastName: Doe
email: john.doe@example.com
age: 35
active: true
joinDate: 2022-03-15
department: Engineering
salary: 85000`,
    flatCsv: `id,firstName,lastName,email,age,active,joinDate,department,salary
1001,John,Doe,john.doe@example.com,35,true,2022-03-15,Engineering,85000`
};

const toast = document.getElementById('apiErrorToast');
let isExpanded = false;

function showError(message) {
    toast.textContent = message;
    toast.style.display = 'block';
    toast.style.whiteSpace = 'nowrap';
    toast.style.overflow = 'hidden';
    toast.style.textOverflow = 'ellipsis';
    toast.style.maxHeight = '3rem';
    isExpanded = false;
}

toast.addEventListener('click', () => {
    if (!isExpanded) {
        toast.style.whiteSpace = 'normal';
        toast.style.overflow = 'visible';
        toast.style.textOverflow = 'clip';
        toast.style.maxHeight = '20rem';
        isExpanded = true;
    } else {
        toast.style.whiteSpace = 'nowrap';
        toast.style.overflow = 'hidden';
        toast.style.textOverflow = 'ellipsis';
        toast.style.maxHeight = '3rem';
        isExpanded = false;
    }
});

function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop()
        .split(';')
        .shift();
}

function getThemePreference() {
    // Try to get theme from localStorage first (more reliable)
    const localTheme = localStorage.getItem('theme');
    if (localTheme) {
        return localTheme;
    }
    
    // Fall back to cookies if localStorage is not available
    const cookieTheme = getCookie('theme');
    if (cookieTheme) {
        return cookieTheme;
    }
    
    // Check system preference if no stored preference
    if (window.matchMedia && window.matchMedia('(prefers-color-scheme: light)').matches) {
        return 'light';
    }
    
    // Default to dark mode
    return 'dark';
}

function setTheme(theme, editorsObj) {
    // Adjust both HTML and body elements
    const html = document.documentElement;
    const body = document.body;
    
    if (theme === 'light') {
        html.classList.add('light-mode');
        html.classList.remove('dark-mode');
        body.classList.add('light-mode');
        body.classList.remove('dark-mode');
        // Rest of your light theme code...
    } else {
        html.classList.remove('light-mode');
        html.classList.add('dark-mode');
        body.classList.remove('light-mode');
        body.classList.add('dark-mode');
        // Rest of your dark theme code...
    }
    
    // Save the theme preference
    try {
        localStorage.setItem('theme', theme);
    } catch (e) {
        console.warn('Could not save theme to localStorage', e);
    }
    
    document.cookie = `theme=${theme}; expires=${new Date(Date.now() + 365 * 24 * 60 * 60 * 1000).toUTCString()}; path=/; SameSite=Lax`;
        
    if (theme === 'light') {
        body.classList.add('light-mode');
        body.classList.remove('dark-mode');
        
        // Only update editors if they exist
        if (editorsObj) {
            Object.values(editorsObj).forEach(editorPair => {
                if (editorPair.input && editorPair.output) {
                    editorPair.input.setOption('theme', 'eclipse');
                    editorPair.output.setOption('theme', 'eclipse');
                }
            });
        }
    } else {
        body.classList.remove('light-mode');
        body.classList.add('dark-mode');
        
        // Only update editors if they exist
        if (editorsObj) {
            Object.values(editorsObj).forEach(editorPair => {
                if (editorPair.input && editorPair.output) {
                    editorPair.input.setOption('theme', 'midnight');
                    editorPair.output.setOption('theme', 'midnight');
                }
            });
        }
    }
    
    // Only refresh editors if they exist
    if (editorsObj) {
        Object.values(editorsObj).forEach(editorPair => {
            if (editorPair.input) editorPair.input.refresh();
            if (editorPair.output) editorPair.output.refresh();
        });
    }
}

function initTheme(editorsObj) {
    const theme = getThemePreference();
    setTheme(theme, editorsObj);
    return theme;
}

function createConverter(convertUrl, inputEditor, outputEditor, indentationId = null) {
    return function () {
        const input = inputEditor.getValue().trim();
        
        if (!input) {
            showError("Please enter some input data first.");
            return;
        }
        
        // Dynamically determine content type based on the target conversion
        const contentTypeMap = {
            '/json-to-xml': 'application/json',
            '/json-to-yaml': 'application/json',
            '/json-to-csv': 'application/json',
            '/xml-to-json': 'application/xml',
            '/xml-to-yaml': 'application/xml',
            '/xml-to-csv': 'application/xml',
            '/yaml-to-json': 'application/x-yaml',
            '/yaml-to-xml': 'application/x-yaml',
            '/yaml-to-csv': 'application/x-yaml',
            '/csv-to-json': 'text/csv',
            '/csv-to-xml': 'text/csv',
            '/csv-to-yaml': 'text/csv'
        };

        const contentType = contentTypeMap[new URL(convertUrl, window.location.origin).pathname.replace('/api/convert', '')] || 'text/plain';
        
        let url = convertUrl;
        const options = {
            method: 'POST',
            headers: {
                'Content-Type': contentType
            },
            body: input
        };
        
        // Add indentation parameter if applicable
        if (indentationId) {
            const indentation = document.getElementById(indentationId).value;
            url = `${convertUrl}?indentation=${indentation}`;
        }
        
        fetch(url, options)
            .then(response => {
                if (!response.ok) {
                    return response.text()
                        .then(text => {
                            throw new Error(text || 'Conversion failed');
                        });
                }
                return response.text();
            })
            .then(data => {
                outputEditor.setValue(data);
                outputEditor.scrollTo(0, 0);
                toast.style.display = 'none';
            })
            .catch(error => {
                showError(error.message || 'Unknown error occurred.');
            });
    };
}

document.addEventListener('DOMContentLoaded', function () {
    
    const style = document.createElement('style');
    style.textContent = `
        .CodeMirror {
            height: 300px !important;
            width: 100% !important;
            border: 1px solid #444;
            border-radius: 4px;
            opacity: 1 !important;
            visibility: visible !important;
            display: block !important;
        }
        
        .light-mode .CodeMirror {
            border-color: #ddd;
        }
        
        .converter-panel {
            opacity: 1 !important;
        }
        
        .CodeMirror-scroll {
            min-height: 300px !important;
        }
      
        .CodeMirror-sizer,
        .CodeMirror-gutter,
        .CodeMirror-gutters,
        .CodeMirror-linenumber {
            display: block !important;
            visibility: visible !important;
        }
        
       
        .converter-panel.hidden {
            display: none !important;
        }
        
        .converter-panel.init-visible {
            position: absolute !important;
            left: -9999px !important;
            top: 0 !important;
            display: block !important;
            visibility: visible !important;
            width: 100% !important;
            height: auto !important;
            opacity: 1 !important;
            overflow: visible !important;
            z-index: -1 !important;
        }
    `;
    document.head.appendChild(style);
    
    function initCodeMirror(elementId, mode, readOnly = false) {
        const textarea = document.getElementById(elementId);
        if (!textarea) {
            console.error(`Element with ID '${elementId}' not found.`);
            return null;
        }
        
        const panel = textarea.closest('.converter-panel');
        const wasHidden = panel.classList.contains('hidden');
        
        if (wasHidden) {
            panel.classList.remove('hidden');
            panel.classList.add('init-visible');
        }
        
        const editor = CodeMirror.fromTextArea(textarea, {
            mode: mode,
            theme: 'midnight',
            lineNumbers: true,
            matchBrackets: true,
            autoCloseBrackets: true,
            readOnly: readOnly,
            lineWrapping: true,
            viewportMargin: Infinity,
            extraKeys: {
                "Tab": false
            }
        });
        
        editor.setSize("100%", "300px");
        
        setTimeout(() => {
            editor.refresh();
        }, 0);
        
        if (wasHidden) {
            setTimeout(() => {
                panel.classList.remove('init-visible');
                panel.classList.add('hidden');
            }, 200);
        }
        
        return editor;
    }
    
    const tabs = document.querySelectorAll('.converter-tab');
    const panels = document.querySelectorAll('.converter-panel');
    
    panels.forEach(panel => {
        if (panel.id !== 'json-xml') {
            panel.classList.add('hidden');
        }
    });
    
    const editors = {
        jsonXml: {
            input: initCodeMirror('jsonXmlInput', 'application/json'),
            output: initCodeMirror('jsonXmlOutput', 'application/xml', true)
        },
        jsonYaml: {
            input: initCodeMirror('jsonYamlInput', 'application/json'),
            output: initCodeMirror('jsonYamlOutput', 'text/x-yaml', true)
        },
        jsonCsv: {
            input: initCodeMirror('jsonCsvInput', 'application/json'),
            output: initCodeMirror('jsonCsvOutput', 'text/plain', true)
        },
        xmlYaml: {
            input: initCodeMirror('xmlYamlInput', 'application/xml'),
            output: initCodeMirror('xmlYamlOutput', 'text/x-yaml', true)
        },
        xmlCsv: {
            input: initCodeMirror('xmlCsvInput', 'application/xml'),
            output: initCodeMirror('xmlCsvOutput', 'text/plain', true)
        },
        yamlCsv: {
            input: initCodeMirror('yamlCsvInput', 'text/x-yaml'),
            output: initCodeMirror('yamlCsvOutput', 'text/plain', true)
        }
    };
    
    tabs.forEach(tab => {
        tab.addEventListener('click', function () {
            
            tabs.forEach(t => t.classList.remove('active'));
            
            panels.forEach(p => p.classList.add('hidden'));
            
            this.classList.add('active');
            
            const targetPanelId = this.dataset.target;
            const targetPanel = document.getElementById(targetPanelId);
            targetPanel.classList.remove('hidden');
            
            if (editors[targetPanelId]) {
                
                for (let i = 0; i < 3; i++) {
                    setTimeout(() => {
                        if (editors[targetPanelId]) {
                            editors[targetPanelId].input.refresh();
                            editors[targetPanelId].output.refresh();
                        }
                    }, i * 100);
                }
            }
        });
    });
    
    const panelObserver = new MutationObserver((mutations) => {
        mutations.forEach((mutation) => {
            if (mutation.attributeName === 'class') {
                const panel = mutation.target;
                const panelId = panel.id;
                
                if (!panel.classList.contains('hidden') && editors[panelId]) {
                    
                    for (let i = 0; i < 3; i++) {
                        setTimeout(() => {
                            editors[panelId].input.refresh();
                            editors[panelId].output.refresh();
                        }, i * 100);
                    }
                }
            }
        });
    });
    
    panels.forEach(panel => {
        panelObserver.observe(panel, {
            attributes: true
        });
    });
    
    function createCopyHandler(outputEditor) {
        return function () {
            const content = outputEditor.getValue();
            if (!content.trim()) {
                showError("No output to copy");
                return;
            }
            navigator.clipboard.writeText(content)
                .then(() => {
                    const originalText = this.textContent;
                    this.textContent = "Copied!";
                    setTimeout(() => {
                        this.textContent = originalText;
                    }, 2000);
                })
                .catch(err => {
                    showError('Failed to copy: ' + err);
                });
        };
    }
    
    function createDownloadHandler(outputEditor, defaultName) {
        return function () {
            const content = outputEditor.getValue();
            if (!content.trim()) {
                showError("No output to download");
                return;
            }
            
            const blob = new Blob([content], {
                type: 'text/plain'
            });
            const url = URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = defaultName;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            URL.revokeObjectURL(url);
        };
    }
    
    function handleSampleSelection() {
        document.querySelectorAll('.sample-option').forEach(option => {
            option.addEventListener('click', function() {
                const sampleType = this.dataset.sampleType; // 'structured' or 'flat'
                const format = this.dataset.format; // 'json', 'xml', 'yaml', or 'csv'
                const converterId = this.closest('.sample-options').id.replace('SampleOptions', '');
                
                const editor = editors[converterId].input;
                const sampleKey = `${sampleType}${format.charAt(0).toUpperCase() + format.slice(1)}`;
                
                editor.setValue(sampleData[sampleKey]);
                editor.refresh();
            });
        });
    }
    
    function createClearHandler(inputEditor, outputEditor) {
        return function () {
            inputEditor.setValue('');
            outputEditor.setValue('');
            inputEditor.refresh();
            outputEditor.refresh();
        };
    }
    
    document.getElementById('themeToggle')
        .addEventListener('click', () => {
            const body = document.body;
            const currentTheme = body.classList.contains('light-mode') ? 'light' : 'dark';
            const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
            setTheme(newTheme, editors);
        });
    
    document.getElementById('openSwaggerBtn')
        .addEventListener('click', () => {
            window.open('/swagger', '_blank');
        });
    
    document.getElementById('convertJsonToXml')
        .addEventListener('click',
            createConverter('/api/convert/json-to-xml',
                editors.jsonXml.input,
                editors.jsonXml.output,
                'jsonXmlIndent'
            )
        );
    document.getElementById('convertXmlToJson')
        .addEventListener('click',
            createConverter('/api/convert/xml-to-json',
                editors.jsonXml.input,
                editors.jsonXml.output,
                'jsonXmlIndent'
            )
        );
    
    document.getElementById('convertJsonToYaml')
        .addEventListener('click',
            createConverter('/api/convert/json-to-yaml',
                editors.jsonYaml.input,
                editors.jsonYaml.output,
                'jsonYamlIndent'
            )
        );
    document.getElementById('convertYamlToJson')
        .addEventListener('click',
            createConverter('/api/convert/yaml-to-json',
                editors.jsonYaml.input,
                editors.jsonYaml.output,
                'jsonYamlIndent'
            )
        );
    
    document.getElementById('convertJsonToCsv')
        .addEventListener('click',
            createConverter('/api/convert/json-to-csv',
                editors.jsonCsv.input,
                editors.jsonCsv.output
            )
        );
    document.getElementById('convertCsvToJson')
        .addEventListener('click',
            createConverter('/api/convert/csv-to-json',
                editors.jsonCsv.input,
                editors.jsonCsv.output
            )
        );
    
    document.getElementById('convertXmlToYaml')
        .addEventListener('click',
            createConverter('/api/convert/xml-to-yaml',
                editors.xmlYaml.input,
                editors.xmlYaml.output,
                'xmlYamlIndent'
            )
        );
    document.getElementById('convertYamlToXml')
        .addEventListener('click',
            createConverter('/api/convert/yaml-to-xml',
                editors.xmlYaml.input,
                editors.xmlYaml.output,
                'xmlYamlIndent'
            )
        );
    
    document.getElementById('convertXmlToCsv')
        .addEventListener('click',
            createConverter('/api/convert/xml-to-csv',
                editors.xmlCsv.input,
                editors.xmlCsv.output
            )
        );
    document.getElementById('convertCsvToXml')
        .addEventListener('click',
            createConverter('/api/convert/csv-to-xml',
                editors.xmlCsv.input,
                editors.xmlCsv.output
            )
        );
    
    document.getElementById('convertYamlToCsv')
        .addEventListener('click',
            createConverter('/api/convert/yaml-to-csv',
                editors.yamlCsv.input,
                editors.yamlCsv.output
            )
        );
    document.getElementById('convertCsvToYaml')
        .addEventListener('click',
            createConverter('/api/convert/csv-to-yaml',
                editors.yamlCsv.input,
                editors.yamlCsv.output
            )
        );

    // Initialize dropdown toggle and sample selection events
    document.querySelectorAll('.sample-loader-dropdown button').forEach(button => {
        button.addEventListener('click', function(e) {
            // Find the dropdown associated with this button
            const dropdown = this.nextElementSibling;
            
            // Toggle the display of the dropdown
            if (dropdown.style.display === 'block') {
                dropdown.style.display = 'none';
            } else {
                // First, close all other dropdowns
                document.querySelectorAll('.sample-options').forEach(menu => {
                    menu.style.display = 'none';
                });
                dropdown.style.display = 'block';
            }
            
            // Prevent event from propagating to document click handler
            e.stopPropagation();
        });
    });

    // Close dropdowns when clicking elsewhere on the page
    document.addEventListener('click', function() {
        document.querySelectorAll('.sample-options').forEach(dropdown => {
            dropdown.style.display = 'none';
        });
    });

    // Prevent clicks inside dropdown from closing it
    document.querySelectorAll('.sample-options').forEach(dropdown => {
        dropdown.addEventListener('click', function(e) {
            e.stopPropagation();
        });
    });

    // Handle sample selection for sample options
    document.querySelectorAll('.sample-option').forEach(option => {
        option.addEventListener('click', function() {
            console.log('Sample option clicked!'); // Debugging
            
            const sampleType = this.dataset.sampleType; // 'structured' or 'flat'
            const format = this.dataset.format; // 'json', 'xml', 'yaml', or 'csv'
            
            // Get converter ID from parent sample-options ID
            const optionsId = this.closest('.sample-options').id;
            console.log('Options ID:', optionsId); // Debugging
            
            // Extract the converter id part (e.g., 'jsonXml' from 'jsonXmlSampleOptions')
            const converterId = optionsId.replace('SampleOptions', '');
            console.log('Converter ID:', converterId); // Debugging
            
            // Build the sample key (e.g., 'flatXml')
            const sampleKey = `${sampleType}${format.charAt(0).toUpperCase() + format.slice(1)}`;
            console.log('Sample key:', sampleKey); // Debugging
            console.log('Sample data available:', Object.keys(sampleData)); // Debugging
            
            // Check if editor exists
            if (!editors[converterId] || !editors[converterId].input) {
                console.error('Editor not found for converter ID:', converterId);
                showError(`Editor not found for ${converterId}`);
                return;
            }
            
            // Check if sample data exists
            if (!sampleData[sampleKey]) {
                console.error('Sample data not found for key:', sampleKey);
                showError(`Sample data not found for ${sampleKey}`);
                return;
            }
            
            // Set the sample data in the editor
            const editor = editors[converterId].input;
            editor.setValue(sampleData[sampleKey]);
            editor.refresh();
            
            // Hide the dropdown
            this.closest('.sample-options').style.display = 'none';
            
            console.log('Sample applied successfully'); // Debugging
        });
    });
    
    document.getElementById('copyJsonXmlOutput')
        .addEventListener('click',
            createCopyHandler(editors.jsonXml.output)
        );
    document.getElementById('copyJsonYamlOutput')
        .addEventListener('click',
            createCopyHandler(editors.jsonYaml.output)
        );
    document.getElementById('copyJsonCsvOutput')
        .addEventListener('click',
            createCopyHandler(editors.jsonCsv.output)
        );
    document.getElementById('copyXmlYamlOutput')
        .addEventListener('click',
            createCopyHandler(editors.xmlYaml.output)
        );
    document.getElementById('copyXmlCsvOutput')
        .addEventListener('click',
            createCopyHandler(editors.xmlCsv.output)
        );
    document.getElementById('copyYamlCsvOutput')
        .addEventListener('click',
            createCopyHandler(editors.yamlCsv.output)
        );
    
    document.getElementById('downloadJsonXmlOutput')
        .addEventListener('click',
            createDownloadHandler(editors.jsonXml.output, 'converted_data.txt')
        );
    document.getElementById('downloadJsonYamlOutput')
        .addEventListener('click',
            createDownloadHandler(editors.jsonYaml.output, 'converted_data.txt')
        );
    document.getElementById('downloadJsonCsvOutput')
        .addEventListener('click',
            createDownloadHandler(editors.jsonCsv.output, 'converted_data.csv')
        );
    document.getElementById('downloadXmlYamlOutput')
        .addEventListener('click',
            createDownloadHandler(editors.xmlYaml.output, 'converted_data.txt')
        );
    document.getElementById('downloadXmlCsvOutput')
        .addEventListener('click',
            createDownloadHandler(editors.xmlCsv.output, 'converted_data.csv')
        );
    document.getElementById('downloadYamlCsvOutput')
        .addEventListener('click',
            createDownloadHandler(editors.yamlCsv.output, 'converted_data.csv')
        );
    
    document.getElementById('clearJsonXmlInput')
        .addEventListener('click',
            createClearHandler(editors.jsonXml.input, editors.jsonXml.output)
        );
    document.getElementById('clearJsonYamlInput')
        .addEventListener('click',
            createClearHandler(editors.jsonYaml.input, editors.jsonYaml.output)
        );
    document.getElementById('clearJsonCsvInput')
        .addEventListener('click',
            createClearHandler(editors.jsonCsv.input, editors.jsonCsv.output)
        );
    document.getElementById('clearXmlYamlInput')
        .addEventListener('click',
            createClearHandler(editors.xmlYaml.input, editors.xmlYaml.output)
        );
    document.getElementById('clearXmlCsvInput')
        .addEventListener('click',
            createClearHandler(editors.xmlCsv.input, editors.xmlCsv.output)
        );
    document.getElementById('clearYamlCsvInput')
        .addEventListener('click',
            createClearHandler(editors.yamlCsv.input, editors.yamlCsv.output)
        );
    
    initTheme();
    
    for (let i = 1; i <= 3; i++) {
        setTimeout(() => {
            
            const activePanel = document.querySelector('.converter-panel:not(.hidden)');
            if (activePanel && editors[activePanel.id]) {
                editors[activePanel.id].input.refresh();
                editors[activePanel.id].output.refresh();
            }
            
            Object.values(editors)
                .forEach(editorPair => {
                    editorPair.input.refresh();
                    editorPair.output.refresh();
                });
        }, i * 300);
    }
    
    window.matchMedia('(prefers-color-scheme: dark)')
        .addEventListener('change', (e) => {
            if (!getCookie('theme')) {
                setTheme(e.matches ? 'dark' : 'light', editors);
            }
        });
    
    setTimeout(() => {
        const activeTab = document.querySelector('.converter-tab.active');
        if (activeTab) {
            const targetPanelId = activeTab.dataset.target;
            const targetPanel = document.getElementById(targetPanelId);
            
            if (targetPanel) {
                
                targetPanel.classList.remove('hidden');
                
                if (editors[targetPanelId]) {
                    editors[targetPanelId].input.refresh();
                    editors[targetPanelId].output.refresh();
                }
            }
        }
    }, 500);

    // Initialize sample dropdown selection handlers
    handleSampleSelection();
});