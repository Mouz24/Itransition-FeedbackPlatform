const AutocompleteQuill = ({ value, onChange, tags }) => {
    const [editorHtml, setEditorHtml] = useState(value);
    const [tagInput, setTagInput] = useState('');
    const [suggestions, setSuggestions] = useState([]);
  
    useEffect(() => {
      setEditorHtml(value);
    }, [value]);
  
    const handleEditorChange = (html) => {
      setEditorHtml(html);
      onChange(html);
    };
  
    const handleTagInputChange = (value) => {
      setTagInput(value);
  
      // Filter tag suggestions based on user input
      const filteredSuggestions = tags.filter((tag) =>
        tag.toLowerCase().includes(value.toLowerCase())
      );
  
      setSuggestions(filteredSuggestions);
    };
  
    const handleTagSelection = (selectedTag) => {
      // Append the selectedTag to the editor content
      const updatedHtml = `${editorHtml} #${selectedTag} `;
      setEditorHtml(updatedHtml);
      onChange(updatedHtml);
      setTagInput('');
    };
  
    return (
      <div>
        <ReactQuill value={editorHtml} onChange={handleEditorChange} />
        <TextInput
          options={tags}
          trigger={['#']}
          value={tagInput}
          onChange={handleTagInputChange}
          onSelect={handleTagSelection}
        />
      </div>
    );
  };